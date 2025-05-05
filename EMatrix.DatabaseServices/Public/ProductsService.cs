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
            .ThenInclude(c => c.Children)
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
                Options = menuItem.Options.Select(o => o.Name).ToList(),
            });
        }
        return model;
    }

    public async Task<ProductsSecondaryViewModel> GetSecondaryViewByMenuItemId(int id)
    {
       var menuItem = await _context.MenuItems
           .FirstOrDefaultAsync(m => m.Id == id);
       var model = new ProductsSecondaryViewModel()
       {

       };
       return model;
    }

    public Task<ProductsSecondaryViewModel> GetSecondaryViewByOptionId(int optionId)
    {
        throw new NotImplementedException();
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