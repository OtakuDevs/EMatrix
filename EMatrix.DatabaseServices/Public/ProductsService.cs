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
            .ThenInclude(mi => mi.MenuItemCategories)
            .ThenInclude(mic => mic.Category)
            .Include(m => m.MenuItems)
            .ThenInclude(mi => mi.MenuItemSubCategories)
            .ThenInclude(misc => misc.SubCategory)
            .Include(m => m.MenuItems)
            .ThenInclude(mi => mi.SubGroupSets)
            .ThenInclude(sgs => sgs.Entries)
            .FirstOrDefaultAsync(m => m.Id == id);

        var model = new ProductsPrimaryViewModel();
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

    public async Task<ProductsSecondaryViewModel> GetSecondaryViewByMenuItemId(int id)
    {
       var menuItem = await _context.MenuItems
           .Include(c => c.MenuItemCategories)
           .ThenInclude(mic => mic.Category)
           .Include(sc => sc.MenuItemSubCategories)
           .ThenInclude(sm => sm.SubCategory)
           .Include(st => st.SubGroupSets)
           .ThenInclude(sgs => sgs.Entries)
           .FirstOrDefaultAsync(m => m.Id == id);
       var model = new ProductsSecondaryViewModel()
       {
           Title = menuItem.Name,
           Categories = menuItem.MenuItemCategories.ToDictionary(c => c.CategoryId, c => c.Category.Alias),
           SubCategories = menuItem.MenuItemSubCategories.ToDictionary(c => c.SubCategoryId, c => c.SubCategory.Alias),
           Sets = menuItem.SubGroupSets.ToDictionary(r => r.Id.ToString(), r => r.Name),
           Mode = SecondaryViewMode.MenuItem
       };
       return model;
    }

    public async Task<ProductsSecondaryViewModel> GetSecondaryViewByCategoryId(string id)
    {
        var category = await _context.Categories
            .Include(c => c.SubCategories)
            .FirstOrDefaultAsync(c => c.Id == id);
        var model = new ProductsSecondaryViewModel()
        {
            Title = category.Name,
            SubCategories = category.SubCategories.ToDictionary(c => c.Id, c => c.Alias),
            Mode = SecondaryViewMode.Category
        };
        return model;
    }

    public async Task<ProductsSecondaryViewModel> GetSecondaryViewBySubCategoryId(string id)
    {
        var subCategory = await _context.SubCategories
            .FirstOrDefaultAsync(c => c.Id == id);
        var model = new ProductsSecondaryViewModel()
        {
            Title = subCategory.Name,
            Mode = SecondaryViewMode.SubCategory
        };
        return model;
    }

    public async Task<ProductsSecondaryViewModel> GetSecondaryViewBySubGroupSetId(int id)
    {
        var set = await _context.MenuItemSubGroupSets
            .Include(s => s.Entries)
            .FirstOrDefaultAsync(s => s.Id == id);
        var model = new ProductsSecondaryViewModel()
        {
            Title = set.Name,
            Sets = set.Entries.ToDictionary(s => s.SubCategoryId, s => s.SubCategoryName),
            Mode = SecondaryViewMode.Set
        };
        return model;
    }
}