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

    public async Task<ProductsPrimaryViewModel> GetPrimaryViewAsync(int id)
    {
        var menu = await _context.Menus
            .Include(m => m.MenuItems)
            .ThenInclude(o => o.Options)
            .FirstOrDefaultAsync(m => m.Id == id);

        var model = new ProductsPrimaryViewModel();
        foreach (var menuItem in menu.MenuItems.OrderBy(m => m.Order))
        {
            model.Menu.Add(new MenuItemViewModel()
            {
                Id = menuItem.Id,
                Name = menuItem.Name,
                Options = menuItem.Options.ToDictionary(o => o.Id, o => o.Name)
            });
            model.MenuItemsGrid.Add(new MenuItemPreviewModel()
            {
                Id = menuItem.Id,
                Name = menuItem.Name,
                Icon = $"{menuItem.Icon}",
                Options = menuItem.Options.ToDictionary(c => c.Id, c => c.Name)
            });
        }

        return model;
    }

    public async Task<ProductsSecondaryViewModel> GetSecondaryViewByMenuItemId(int id)
    {
        var menuItem = await _context.MenuItems
            .Include(m => m.Options)
            .ThenInclude(m => m.Children)
            .FirstOrDefaultAsync(m => m.Id == id);
        var model = new ProductsSecondaryViewModel()
        {
            OptionId = -1,
            Title = menuItem.Name,
            Options = menuItem.Options
                .OrderBy(c => c.Order)
                .Select(s => new OptionChildViewModel()
                {
                    Id = s.Id,
                    Name = s.Name,
                    Entries = s.Children
                        .OrderBy(c => c.DisplayName)
                        .ToDictionary(c => c.Id.ToString(), c => c.DisplayName)
                })
                .ToList()
        };
        return model;
    }

    public async Task<ProductsSecondaryViewModel> GetSecondaryViewByOptionId(int optionId)
    {
        var option = await _context.MenuOptions
            .Include(m => m.Children)
            .ThenInclude(sc => sc.SubGroup)
            .Include(m => m.Children)
            .ThenInclude(ss => ss.SubGroupSet)
            .ThenInclude(sc => sc.Items)
            .ThenInclude(sc => sc.SubGroup)
            .FirstOrDefaultAsync(m => m.Id == optionId);
        if (option == null)
            throw new KeyNotFoundException();
        var model = new ProductsSecondaryViewModel()
        {
            OptionId = option.Id,
            Title = option.Name,
            Options = option.Children.OrderBy(c => c.DisplayName)
                .Select(s => new OptionChildViewModel()
                {
                    Id = s.Id,
                    Name = s.DisplayName,
                    SubGroupId = s.SubGroup == null ? null : s.SubGroup.Id.ToString(),
                    Entries = s.SubGroupSet == null ? null : s.SubGroupSet.Items.ToDictionary(sc => sc.SubGroupId, sc => sc.SubGroup.Alias)
                })
                .ToList()
        };
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