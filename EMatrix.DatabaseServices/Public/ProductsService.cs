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
                .ToListAsync();
            foreach (var item in menuItems)
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
        }
        return model;
    }

    public async Task<ProductsSecondaryViewModel> GetSecondaryViewAsync(string id, int optionId)
    {
        var items = await _context.InventoryItems
            .Include(s => s.SubCategory)
            .Where(s => s.SubCategoryId == id)
            .ToListAsync();
        var model = new ProductsSecondaryViewModel();
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


    // Helper function to extract the group key (i.e., prefix before first space)
    private string GetGroupKey(string name)
    {
        if (string.IsNullOrEmpty(name)) return "";

        // Split the name by space and take the first part (this is our dynamic prefix)
        var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length > 0 ? parts[0].ToUpper() : name.ToUpper();
    }
}