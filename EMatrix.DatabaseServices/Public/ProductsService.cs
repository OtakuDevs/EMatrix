using EMatrix.Database;
using EMatrix.DatabaseServices.Public.Interfaces;
using EMatrix.DataModels;
using EMatrix.ViewModels.Products;
using Microsoft.EntityFrameworkCore;

namespace EMatrix.DatabaseServices.Public;

public class ProductsService : IProductsService
{
    private readonly EMatrixDbContext _context;

    public ProductsService(EMatrixDbContext context)
    {
        _context = context;
    }

    public async Task<ProductsPrimaryViewModel> GetPrimaryViewAsync(int id, string type)
    {
        ProductsPrimaryViewModel model = new ProductsPrimaryViewModel();
        switch (type)
        {
            case "MenuItem":
            {
                var menuItems = await GetMenuItemsAsync();
                model.Accordion = GetAccordionForMenuItemAsync(menuItems, type);
                model.MenuPreview = GetMenuPreviewForMenuItemAsync(menuItems, type);
                break;
            }
            case "Option":
            {
                model.Accordion.Type = type;
                var option = await GetMenuOptionAsync(id);

                model.Accordion = GetAccordionForMenuOptionAsync(option, type);
                model.MenuPreview = GetMenuPreviewForMenuOptionAsync(option, type);
                break;
            }
        }

        return model;
    }

    public async Task<ProductsSecondaryViewModel> GetSecondaryViewAsync(string id, int childId, int optionId,
        int page = 1, string search = "")
    {
        const int pageSize = 8;
        var skip = (page - 1) * pageSize;

        //Base query
        var query = _context.InventoryItems
            .Include(s => s.SubCategory)
            .Where(s => s.SubCategoryId == id)
            .OrderBy(i => i.NameAlias)
            .AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            string[] tokens = search
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.ToLower())
                .ToArray();

            query = query.Where(item =>

                // every token must be found in *one* of the text columns
                tokens.All(t =>
                    item.NameAlias.ToLower().Contains(t) ||
                    item.DescriptionAlias.ToLower().Contains(t) ||
                    item.SubCategory.Alias.ToLower().Contains(t) ||
                    item.Category.Alias.ToLower().Contains(t)
                )
                || EF.Functions.Like(item.Id, $"%{search}%")
            ).OrderBy(i => i.NameAlias);
        }

        var totalPages = (int)Math.Ceiling((double)query.Count() / pageSize);

        var items = await query.Skip(skip).Take(pageSize).ToListAsync();
        var selectedSubGroup = await _context.SubCategories.FirstOrDefaultAsync(s => s.Id == id);

        var option = await GetMenuOptionAsync(optionId);
        var accordion = GetAccordionForMenuOptionAsync(option, "Option");

        var childIcon = option.Children.FirstOrDefault(o => o.Id == childId)!.Icon;
        var products = items.OrderBy(i => i.NameAlias)
            .Select(i => new ProductListingViewModel()
            {
                Id = i.Id,
                SubCategory = i.SubCategory.Alias,
                NameAlias = i.NameAlias,
                Icon = childIcon,
                Price = i.Price,
                Availability = i.Quantity > 0
            }).ToList();

        var model = new ProductsSecondaryViewModel()
        {
            Accordion = new AccordionViewModel()
            {
                Type = "Option",
                SelectedSubGroupId = selectedSubGroup!.Id,
                SelectedSubGroupName = selectedSubGroup.Alias,
                SelectedChildId = childId,
                Options = accordion.Options,
            },
            CurrentPage = page,
            TotalPages = totalPages,
            SearchTerm = search,
            Products = products,
        };

        return model;
    }

    public async Task<ProductsSearchViewModel> GetSearchViewAsync(int? optionId, string type, string search,
        int page = 1)
    {
        const int pageSize = 12;
        var skip = (page - 1) * pageSize;

        //Base query
        var query = _context.InventoryItems
            .Include(s => s.SubCategory)
            .Include(c => c.Category)
            .AsQueryable();

        var model = new ProductsSearchViewModel();

        switch (type)
        {
            case "MenuItem":
            {
                var menuItems = await GetMenuItemsAsync();
                model.Accordion = GetAccordionForMenuItemAsync(menuItems, type);
                break;
            }
            case "Option":
            {
                var option = await GetMenuOptionAsync(optionId!.Value);
                model.Accordion = GetAccordionForMenuOptionAsync(option, type);

                //When searching in option, filter items only from that option
                var optionSubgroupIds = option.Children
                    .SelectMany(child =>
                    {
                        if (child.SubGroupId != null)
                        {
                            return new[] { child.SubGroupId };
                        }
                        if (child.SubGroupSetId != null)
                        {
                            return child.SubGroupSet!.Items.Select(i => i.SubGroupId);
                        }

                        return new List<string>();
                    })
                    .Distinct()
                    .ToList();
                query = query.Where(o => optionSubgroupIds.Contains(o.SubCategoryId));
                break;
            }
        }

        string[] tokens = search
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(t => t.ToLower())
            .ToArray();

        query = query.Where(item =>

            // every token must be found in *one* of the text columns
            tokens.All(t =>
                item.NameAlias.ToLower().Contains(t) ||
                item.DescriptionAlias.ToLower().Contains(t) ||
                item.SubCategory.Alias.ToLower().Contains(t) ||
                item.Category.Alias.ToLower().Contains(t)
            )
            || EF.Functions.Like(item.Id, $"%{search}%")
        ).OrderBy(i => i.NameAlias);

        var totalPages = (int)Math.Ceiling((double)query.Count() / pageSize);

        var items = await query.Skip(skip).Take(pageSize).ToListAsync();

        var menuOptionChildren = _context.MenuOptionChildren
            .Include(c => c.SubGroupSet)
            .ThenInclude(sc => sc.Items)
            .ThenInclude(i => i.SubGroup)
            .ToList();

        model.Products = items
            .OrderBy(i => i.NameAlias)
            .AsEnumerable() // switch to LINQ to Objects
            .Select(i => new ProductListingViewModel()
            {
                Id = i.Id,
                SubCategory = i.SubCategory.Alias,
                NameAlias = i.NameAlias,
                Icon = menuOptionChildren
                    .FirstOrDefault(c =>
                        (c.SubGroupId != null && c.SubGroupId == i.SubCategoryId) ||
                        (c.SubGroupSetId != null && c.SubGroupSet.Items.Any(s => s.SubGroupId == i.SubCategoryId)))
                    ?.Icon,
                Price = i.Price,
                Availability = i.Quantity > 0
            })
            .ToList();
        model.CurrentPage = page;
        model.TotalPages = totalPages;
        model.SearchTerm = search;
        return model;
    }

    public async Task<ProductDetailsViewModel> GetDetailsViewAsync(string id)
    {
        var item = await _context.InventoryItems
            .Include(i => i.SubCategory)
            .Include(c => c.Category)
            .FirstOrDefaultAsync(i => i.Id == id);
        string[] tokens = item.Description
            .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(t => t.ToLowerInvariant().Trim())
            .ToArray();

        var patterns = tokens.Select(t => $";{t};").ToArray();

// Fetch possible candidates with any match
        var potentialMatches = await _context.InventoryItems
            .Include(i => i.SubCategory)
            .Include(i => i.Category)
            .Where(i => i.CategoryId == item.CategoryId &&
                        i.Id != item.Id &&
                        patterns.Any(p => (";" + i.Description.ToLower() + ";").Contains(p)))
            .ToListAsync(); // move to memory here

// Now filter for at least 3 matches
        var similarProducts = potentialMatches
            .Where(i =>
            {
                string description = ";" + i.Description.ToLower() + ";";
                int matchCount = patterns.Count(p => description.Contains(p));
                return matchCount >= 3;
            })
            .OrderBy(i => i.NameAlias)
            .Select(i => new ProductListingViewModel
            {
                Id = i.Id,
                SubCategory = i.SubCategory.Alias,
                NameAlias = i.NameAlias,
                Icon =  _context.MenuOptionChildren
                    .FirstOrDefault(c => c.SubGroupId != null && c.SubGroupId == i.SubCategoryId
                    || c.SubGroupSetId != null && c.SubGroupSet.Items.Any(s => s.SubGroupId == i.SubCategoryId)).Icon,
                Price = i.Price,
                Availability = i.Quantity > 0
            })
            .ToList();

        var menuOptionChild = _context.MenuOptionChildren
            .Include(o => o.MenuOption)
            .FirstOrDefault(c =>
                (c.SubGroupId != null && c.SubGroupId == item.SubCategoryId) ||
                (c.SubGroupSetId != null && c.SubGroupSet.Items.Any(s => s.SubGroupId == item.SubCategoryId)));
        if (menuOptionChild == null)
        {
            var menuItems = await GetMenuItemsAsync();
            var accordionDefault = GetAccordionForMenuItemAsync(menuItems, "MenuItem");

            var modelDefault = new ProductDetailsViewModel()
            {
                Id = item.Id,
                Name = item.NameAlias,
                Description = item.DescriptionAlias.Split(';', StringSplitOptions.RemoveEmptyEntries)          // "k : v" pieces
                    .Select(piece =>
                    {
                        var parts = piece.Split(':', 2, StringSplitOptions.TrimEntries); // split on FIRST ':'
                        return (Key: parts[0], Value: parts.Length > 1 ? parts[1] : "");
                    })
                    .ToDictionary(x => x.Key, x => x.Value),
                Icon = "/images/default/placeholder.png",
                Price = item.Price,
                Availability = item.Quantity > 0,
                Unit = item.Unit,
                Accordion = accordionDefault,
                SimilarProducts = similarProducts
            };
            return modelDefault;
        }
        var option = await GetMenuOptionAsync(menuOptionChild.MenuOption.Id);
        var accordion = GetAccordionForMenuOptionAsync(option, "Option");

        var model = new ProductDetailsViewModel()
        {
            Id = item.Id,
            Name = item.NameAlias,
            Description = item.DescriptionAlias.Split(';', StringSplitOptions.RemoveEmptyEntries)          // "k : v" pieces
                .Select(piece =>
                {
                    var parts = piece.Split(':', 2, StringSplitOptions.TrimEntries); // split on FIRST ':'
                    return (Key: parts[0], Value: parts.Length > 1 ? parts[1] : "");
                })
                .ToDictionary(x => x.Key, x => x.Value),
            Icon = menuOptionChild.Icon,
            Price = item.Price,
            Availability = item.Quantity > 0,
            Unit = item.Unit,
            ProductMenuOption = new OptionModel()
            {
                Id = menuOptionChild.MenuOption.Id,
                Name = menuOptionChild.MenuOption.Name
            },
            ProductMenuOptionChild = new OptionModel()
            {
                Id = menuOptionChild.Id,
                Name = menuOptionChild.DisplayName,
                SubGroupId = item.SubCategoryId
            },
            Accordion = new AccordionViewModel()
            {
                Type = accordion.Type,
                SelectedSubGroupId = item.SubCategoryId,
                SelectedSubGroupName = item.SubCategory.Alias,
                SelectedChildId = menuOptionChild.Id,
                Options = accordion.Options,
            },
            SimilarProducts = similarProducts
        };

        return model;
    }

    private AccordionViewModel GetAccordionForMenuItemAsync(List<MenuItem> menuItems, string type)
    {
        var options = menuItems
            .OrderBy(o => o.Order)
            .Select(o => new AccordionOptionViewModel()
            {
                Name = o.Name,
                Options = o.Options.Select(opt => new AccordionOptionChildViewModel()
                {
                    Id = opt.Id,
                    Name = opt.Name,
                }).ToList(),
            })
            .ToList();
        return new AccordionViewModel() { Type = type, Options = options };
    }

    private AccordionViewModel GetAccordionForMenuOptionAsync(MenuOption option, string type)
    {
        var accOption = new AccordionOptionViewModel()
        {
            Id = option.Id,
            Name = option.Name,
            Options = option.Children.Select(o => new AccordionOptionChildViewModel()
                {
                    Id = o.Id,
                    Name = o.DisplayName ?? "Placeholder",
                    SubGroupId = o.SubGroupId,
                    Entries = o.SubGroupSetId == null
                        ? null
                        : o.SubGroupSet!.Items.ToDictionary(sc => sc.SubGroupId, sc => sc.SubGroup.Alias)
                })
                .OrderBy(o => o.Name)
                .ToList(),
        };
        var model = new AccordionViewModel()
        {
            Type = type,
            Options = new List<AccordionOptionViewModel>(){ accOption }
        };
        return model;
    }

    private MenuPreviewViewModel GetMenuPreviewForMenuItemAsync(List<MenuItem> menuItems, string type)
    {
        var options = menuItems
            .OrderBy(o => o.Order)
            .Select(o => new MenuOptionViewModel()
            {
                Id = o.Id,
                Name = o.Name,
                Icon = o.Icon,
                Children = o.Options.ToDictionary(c => c.Id.ToString(), c => c.Name)
            }).ToList();
        return new MenuPreviewViewModel() { Type = type, Options = options };
    }

    private MenuPreviewViewModel GetMenuPreviewForMenuOptionAsync(MenuOption option, string type)
    {
        var options = option.Children.Select(o => new MenuOptionViewModel()
            {
                Id = o.Id,
                Name = o.DisplayName ?? "Placeholder",
                Icon = !string.IsNullOrWhiteSpace(o.Icon) ? o.Icon : option.MenuItem.Icon,
                Children = o.SubGroupSetId == null
                    ? new Dictionary<string, string> { { o.SubGroup!.Id, o.SubGroup.Name } }
                    : o.SubGroupSet!.Items.ToDictionary(sc => sc.SubGroupId, sc => sc.SubGroup.Alias)
            })
            .OrderBy(o => o.Name)
            .ToList();

        return new MenuPreviewViewModel() { Id = option.Id, Type = type, Options = options };
    }

    private async Task<List<MenuItem>> GetMenuItemsAsync()
    {
        var menuItems = await _context.MenuItems
            .Include(o => o.Options)
            .ThenInclude(c => c.Children)
            .ToListAsync();
        return menuItems;
    }

    private async Task<MenuOption> GetMenuOptionAsync(int optionId)
    {
        var option = await _context.MenuOptions
            .Include(ch => ch.Children)
            .ThenInclude(s => s.SubGroup)
            .Include(ch => ch.Children)
            .ThenInclude(s => s.SubGroupSet)
            .ThenInclude(sc => sc!.Items)
            .ThenInclude(scs => scs.SubGroup)
            .Include(ch => ch.MenuItem)
            .FirstOrDefaultAsync(o => o.Id == optionId);
        return option!;
    }
}