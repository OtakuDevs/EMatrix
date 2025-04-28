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

       };
       return model;
    }

    public async Task<ProductsSecondaryViewModel> GetSecondaryViewByCategoryId(string id)
    {
        var category = await _context.Categories
            .Include(c => c.SubCategories)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null)
            throw new KeyNotFoundException();
        // Create the final model
        var model = new ProductsSecondaryViewModel()
        {
            Title = category.Name,
            Mode = SecondaryViewMode.Category
        };
        // Group subcategories dynamically by prefix (first part of the name)
        var groupedSubCategories = category.SubCategories
            .GroupBy(sub => sub.Alias)
            .ToDictionary(g => g.Key, g => g.ToList());
        foreach (var gsc in groupedSubCategories)
        {
            if(gsc.Value.Count == 1)
                model.SubGroups.Add(new OptionViewModel()
                {
                    Name = gsc.Key,
                    Id = gsc.Value.First().Id,
                });
            else
            {
                model.SubGroups.Add(new OptionViewModel()
                {
                    Name = gsc.Key,
                    Entries = gsc.Value.Select(c => c.Id).ToList()
                });
            }
        }
        model.SubGroups = model.SubGroups.OrderBy(g => g.Name).ToList();

        return model;
    }

    public async Task<ProductsSecondaryViewModel> GetSecondaryViewBySubCategoryId(string id)
    {

        var model = new ProductsSecondaryViewModel()
        {
        };
        return model;
    }

    public async Task<ProductsSecondaryViewModel> GetSecondaryViewBySubGroupSetId(int id)
    {

        var model = new ProductsSecondaryViewModel()
        {

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