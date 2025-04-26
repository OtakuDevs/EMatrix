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
        foreach (var menuItem in menu.MenuItems.OrderBy(m => m.Order))
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
                Icon = $"{menuItem.Icon}",
                Items = menuItem.MenuItemCategories
                    .Select(c => c.Category.Alias)
                    .Concat(menuItem.MenuItemSubCategories.Select(c => c.SubCategory.Alias))
                    .Concat(menuItem.SubGroupSets.Select(r => r.Name))
                    .ToList()
            });
        }
        return model;
    }

    public async Task<MenuItemOptionViewModel> GetMenuItemResultAsync(string? categoryId, string? subCategoryId, int subGroupSetId = 0)
    {
        var model = new MenuItemOptionViewModel();
        if (categoryId != null)
        {
            var category = await _context.Categories
                .Include(c => c.SubCategories)
                .FirstOrDefaultAsync(c => c.Id == categoryId);
            if (category == null)
                throw new KeyNotFoundException($"Category with id {categoryId} not found");
            model.Name = category.Name;
            model.Options = category.SubCategories.ToDictionary(sc => sc.Id, sc => sc.Alias);
            return model;
        }

        if (subCategoryId != null)
        {
            var subCategory = await _context.SubCategories.FirstOrDefaultAsync(s => s.Id == subCategoryId);
            if (subCategory == null)
                throw new KeyNotFoundException($"Category with id {subCategoryId} not found");
            model.Name = subCategory.Name;
            return model;
        }

        if (subGroupSetId != 0)
        {
            var set = await _context.MenuItemSubGroupSets
                .Include(sgs => sgs.Entries)
                .FirstOrDefaultAsync(s => s.Id == subGroupSetId);
            if(set == null)
                throw new KeyNotFoundException($"Sub group set with id {subGroupSetId} not found");
            model.Name = set.Name;
            model.Options = set.Entries.ToDictionary(s => s.SubCategoryId, s => s.SubCategoryName);
            return model;
        }
        throw new KeyNotFoundException($"Category with id {categoryId} not found");
    }
}