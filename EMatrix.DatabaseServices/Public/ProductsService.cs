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

    public async Task<ProductsIndexViewModel> GetProductsAsync(int id)
    {
        var menu = await _context.Menus
            .Include(r => r.MenuItems)
            .ThenInclude(r => r.MenuItemCategories).ThenInclude(c => c.Category)
            .Include(c => c.MenuItems).ThenInclude(c => c.MenuItemSubCategories)
            .ThenInclude(c => c.SubCategory)
            .FirstOrDefaultAsync(r => r.Id == id);
        var model = new ProductsIndexViewModel();
        foreach (var menuItem in menu.MenuItems)
        {
            model.Menu.Add(new MenuItemViewModel()
            {
                Id = menuItem.Id,
                Name = menuItem.Name,
                Categories = menuItem.MenuItemCategories.ToDictionary(c => c.CategoryId, c => c.Category.Alias),
                SubCategories = menuItem.MenuItemSubCategories.ToDictionary(c => c.SubCategoryId, c => c.SubCategory.Alias),
            });
            model.MenuItemsGrid.Add(new MenuItemPreviewModel()
            {
                Id = menuItem.Id,
                Name = menuItem.Name,
                Icon = $"{menuItem.Icon}"
            });
        }
        return model;
    }
}