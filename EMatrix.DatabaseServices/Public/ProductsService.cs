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

    public async Task<ProductsSecondaryViewModel> GetSecondaryViewAsync(string id, int optionId)
    {
        var items = await _context.InventoryItems
            .Include(s => s.SubCategory)
            .Where(s => s.SubCategoryId == id)
            .ToListAsync();
        var selectedSubGroup = await _context.SubCategories.FirstOrDefaultAsync(s => s.Id == id);
        var model = new ProductsSecondaryViewModel();
        model.Accordion.Type = "Option";
        model.Accordion.SelectedSubGroupId = selectedSubGroup.Id;
        model.Accordion.SelectedSubGroupName = selectedSubGroup.Alias;
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
        return model;
    }
}