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
            .Include(m => m.MenuItems)
            .ThenInclude(mi => mi.MenuItemCategories)
            .ThenInclude(mic => mic.Category)
            .Include(m => m.MenuItems)
            .ThenInclude(mi => mi.MenuItemSubCategories)
            .ThenInclude(misc => misc.SubCategory)
            .Include(m => m.MenuItems)
            .ThenInclude(mi => mi.SubGroupSets)
            .ThenInclude(sgs => sgs.Entries)
            .FirstOrDefaultAsync(m => m.Id == id);

        var model = new ProductsIndexViewModel();
        foreach (var menuItem in menu.MenuItems)
        {
            model.Menu.Add(new MenuItemViewModel()
            {
                Id = menuItem.Id,
                Name = menuItem.Name,
                Categories = menuItem.MenuItemCategories.ToDictionary(c => c.CategoryId, c => c.Category.Alias),
                SubCategories = menuItem.MenuItemSubCategories.ToDictionary(c => c.SubCategoryId, c => c.SubCategory.Alias),
                SubGroupSets = menuItem.SubGroupSets.ToDictionary(r => r.Id, r => r.Name)
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