using EMatrix.Database;
using EMatrix.DatabaseServices.Public.Interfaces;
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
        if (type == "MenuItem")
        {
            model.Accordion.Type = type;
            var menuItems = await _context.MenuItems
                .Include(o => o.Options)
                .ThenInclude(c => c.Children)
                .ToListAsync();
            foreach (var item in menuItems.OrderBy(o => o.Order))
            {
                model.Accordion.Options.Add(new AccordionOptionViewModel()
                {
                    Name = item.Name,
                    Options = item.Options.Select(opt => new AccordionOptionChildViewModel()
                    {
                        Id = opt.Id,
                        Name = opt.Name,
                    }).ToList(),
                });
            }

            model.MenuPreview.Type = type;
            model.MenuPreview.Options = menuItems
                .OrderBy(o => o.Order)
                .Select(o => new MenuOptionViewModel()
                {
                    Id = o.Id,
                    Name = o.Name,
                    Icon = o.Icon,
                    Children = o.Options.ToDictionary(c => c.Id.ToString(), c => c.Name)
                }).ToList();
        }
        else if (type == "Option")
        {
            model.Accordion.Type = type;
            var option = await _context.MenuOptions
                .Include(ch => ch.Children)
                .ThenInclude(s => s.SubGroup)
                .Include(ch => ch.Children)
                .ThenInclude(s => s.SubGroupSet)
                .ThenInclude(sc => sc.Items)
                .ThenInclude(sc => sc.SubGroup)
                .Include(ch => ch.MenuItem)
                .FirstOrDefaultAsync(o => o.Id == id);

            model.Accordion.Options.Add(new AccordionOptionViewModel()
            {
                Id = option.Id,
                Name = option.Name,
                Options = option.Children.Select(o => new AccordionOptionChildViewModel()
                    {
                        Id = o.Id,
                        Name = o.DisplayName ?? o.SubGroup?.Name,
                        SubGroupId = o.SubGroupId != null ? o.SubGroupId : null,
                        Entries = o.SubGroupSetId == null
                            ? null
                            : o.SubGroupSet.Items.ToDictionary(sc => sc.SubGroupId, sc => sc.SubGroup.Alias)
                    })
                    .ToList(),
            });
            model.MenuPreview.Id = option.Id;
            model.MenuPreview.Type = type;
            model.MenuPreview.Options = option.Children.Select(o => new MenuOptionViewModel()
                {
                    Id = o.Id,
                    Name = o.DisplayName ?? o.SubGroup?.Name,
                    Icon = !string.IsNullOrWhiteSpace(o.Icon) ? o.Icon : option.MenuItem.Icon,
                    Children = o.SubGroupSetId == null
                        ? new Dictionary<string, string> { { o.SubGroup!.Id, o.SubGroup.Name } }
                        : o.SubGroupSet.Items.ToDictionary(sc => sc.SubGroupId, sc => sc.SubGroup.Alias)
                })
                .ToList();
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
            var normalizedSearch = search.Trim().ToLower();
            query = query.Where(i =>
                    i.NameAlias.ToLower().Contains(normalizedSearch) ||
                    i.DescriptionAlias.ToLower().Contains(normalizedSearch) ||
                    i.SubCategory.Alias.ToLower().Contains(normalizedSearch) ||
                    i.Category.Alias.ToLower().Contains(normalizedSearch))
                .OrderBy(i => i.NameAlias);
        }

        var totalPages = (int)Math.Ceiling((double)query.Count() / pageSize);

        var items = await query.Skip(skip).Take(pageSize).ToListAsync();

        var selectedSubGroup = await _context.SubCategories.FirstOrDefaultAsync(s => s.Id == id);
        var model = new ProductsSecondaryViewModel();
        model.Accordion.Type = "Option";
        model.Accordion.SelectedSubGroupId = selectedSubGroup.Id;
        model.Accordion.SelectedSubGroupName = selectedSubGroup.Alias;
        model.Accordion.SelectedChildId = childId;
        model.CurrentPage = page;
        model.TotalPages = totalPages;
        model.SearchTerm = search;
        var option = await _context.MenuOptions
            .Include(ch => ch.Children)
            .ThenInclude(s => s.SubGroup)
            .Include(ch => ch.Children)
            .ThenInclude(s => s.SubGroupSet)
            .ThenInclude(sc => sc.Items)
            .ThenInclude(sc => sc.SubGroup)
            .FirstOrDefaultAsync(o => o.Id == optionId);
        model.Accordion.Options.Add(new AccordionOptionViewModel()
        {
            Id = option.Id,
            Name = option.Name,
            Options = option.Children.Select(o => new AccordionOptionChildViewModel()
                {
                    Id = o.Id,
                    Name = o.DisplayName ?? o.SubGroup?.Name,
                    SubGroupId = o.SubGroupId != null ? o.SubGroupId : null,
                    Entries = o.SubGroupSetId == null
                        ? null
                        : o.SubGroupSet.Items.ToDictionary(sc => sc.SubGroupId, sc => sc.SubGroup.Alias)
                })
                .ToList(),
        });

        var childIcon = option.Children.FirstOrDefault(o => o.Id == childId).Icon;
        model.Products = items.OrderBy(i => i.NameAlias)
            .Select(i => new ProductListingViewModel()
            {
                Id = i.Id,
                SubCategory = i.SubCategory.Alias,
                NameAlias = i.NameAlias,
                Icon = childIcon,
                Price = i.Price,
                Availability = i.Quantity > 0
            }).ToList();
        return model;
    }

    public async Task<ProductsSearchViewModel> GetSearchViewAsync(int? optionId, string type, string search,
        int page = 1)
    {
        var model = new ProductsSearchViewModel();
        const int pageSize = 12;
        var skip = (page - 1) * pageSize;

        var query = _context.InventoryItems
            .Include(s => s.SubCategory)
            .Include(c => c.Category)
            .AsQueryable();

        if (type == "MenuItem")
        {
            model.Accordion.Type = type;
            var menuItems = await _context.MenuItems
                .Include(o => o.Options)
                .ThenInclude(c => c.Children)
                .ToListAsync();
            foreach (var item in menuItems.OrderBy(o => o.Order))
            {
                model.Accordion.Options.Add(new AccordionOptionViewModel()
                {
                    Name = item.Name,
                    Options = item.Options.Select(opt => new AccordionOptionChildViewModel()
                    {
                        Id = opt.Id,
                        Name = opt.Name,
                    }).ToList(),
                });
            }
        }
        else if (type == "Option")
        {
            model.Accordion.Type = type;
            var option = await _context.MenuOptions
                .Include(ch => ch.Children)
                .ThenInclude(s => s.SubGroup)
                .Include(ch => ch.Children)
                .ThenInclude(s => s.SubGroupSet)
                .ThenInclude(sc => sc.Items)
                .ThenInclude(sc => sc.SubGroup)
                .FirstOrDefaultAsync(o => o.Id == optionId);

            model.Accordion.Options.Add(new AccordionOptionViewModel()
            {
                Id = option.Id,
                Name = option.Name,
                Options = option.Children.Select(o => new AccordionOptionChildViewModel()
                    {
                        Id = o.Id,
                        Name = o.DisplayName ?? o.SubGroup?.Name,
                        SubGroupId = o.SubGroupId != null ? o.SubGroupId : null,
                        Entries = o.SubGroupSetId == null
                            ? null
                            : o.SubGroupSet.Items.ToDictionary(sc => sc.SubGroupId, sc => sc.SubGroup.Alias)
                    })
                    .ToList(),
            });
            var optionSubgroupIds = option.Children
                .SelectMany(child =>
                {
                    if (child.SubGroupId != null)
                    {
                        return new[] { child.SubGroupId };
                    }
                    else if (child.SubGroupSetId != null)
                    {
                        return child.SubGroupSet.Items.Select(i => i.SubGroupId);
                    }

                    return new List<string>();
                })
                .Distinct()
                .ToList();
            query = query.Where(o => optionSubgroupIds.Contains(o.SubCategoryId));
        }

        var normalizedSearch = search.Trim().ToLower();
        query = query.Where(i =>
                i.NameAlias.ToLower().Contains(normalizedSearch) ||
                i.DescriptionAlias.ToLower().Contains(normalizedSearch) ||
                i.SubCategory.Alias.ToLower().Contains(normalizedSearch) ||
                i.Category.Alias.ToLower().Contains(normalizedSearch) ||
                EF.Functions.Like(i.Id, $"{normalizedSearch}%"))
            .OrderBy(i => i.NameAlias);
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
}