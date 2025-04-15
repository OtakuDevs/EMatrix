using EMatrix.Constants;
using EMatrix.Database;
using EMatrix.DatabaseServices.Admin.Interfaces;
using EMatrix.DataModels;
using EMatrix.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EMatrix.DatabaseServices.Admin;

public class MenuManageService : IMenuManageService
{
    private readonly EMatrixDbContext _context;

    public MenuManageService(EMatrixDbContext context)
    {
        _context = context;
    }

    public async Task<MenuAdminViewModel> GetMenu(int id)
    {
        var menu = await _context.Menus
            .Include(r => r.MenuItems).FirstOrDefaultAsync(m => m.Id == id);
        if(menu == null)
            throw new KeyNotFoundException();

        var model = new MenuAdminViewModel()
        {
            MenuItems = menu.MenuItems.ToDictionary(mi => mi.Id, mi => mi.Name),
        };
        return model;
    }

    public async Task<bool> AddMenuItemAsync(string name)
    {
        try
        {
            var menuItem = new MenuItem()
            {
                Name = name,
                MenuId = ConfigurationConstants.MenuId,
                MenuItemCategories = new List<MenuItemCategory>(),
                MenuItemSubCategories = new List<MenuItemSubCategory>(),
            };
            _context.MenuItems.Add(menuItem);
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            return false;
        }
       return true;
    }

    public async Task RenameMenuItemAsync(int id, string name)
    {
        var menuItem = _context.MenuItems.FirstOrDefault(m => m.Id == id);
        if(menuItem == null)
            throw new KeyNotFoundException();
        menuItem.Name = name;
        _context.MenuItems.Update(menuItem);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteMenuItemAsync(int id)
    {
        var menuItem = _context.MenuItems.FirstOrDefault(m => m.Id == id);
        if(menuItem == null)
            throw new KeyNotFoundException();
        _context.MenuItems.Remove(menuItem);
        await _context.SaveChangesAsync();
    }

    public async Task<MenuItemAdminViewModel> GetMenuItemModelAsync(int id)
    {
        var availableCategories = await _context.Categories
            .Select(c => new SelectListItem
            {
                Text = c.Alias,
                Value = c.Id
            })
            .OrderBy(c => c.Value)
            .ToListAsync();

        var availableSubCategories = await _context.SubCategories
            .Select(sc => new SelectListItem
            {
                Text = sc.Alias,
                Value = sc.Id
            })
            .OrderBy(sc => sc.Value) // assuming Value = XXNN format
            .ToListAsync();

        var menuItem = await _context.MenuItems
            .Include(m => m.MenuItemCategories).ThenInclude(mc => mc.Category)
            .Include(m => m.MenuItemSubCategories).ThenInclude(msc => msc.SubCategory)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (menuItem == null)
            throw new KeyNotFoundException();

        var model = new MenuItemAdminViewModel
        {
            Id = menuItem.Id,
            Name = menuItem.Name,
            Categories = menuItem.MenuItemCategories.ToDictionary(mc => mc.CategoryId, mc => mc.Category.Name),
            SubCategories = menuItem.MenuItemSubCategories.ToDictionary(msc => msc.SubCategoryId, msc => msc.SubCategory.Name),
            AvailableCategories = availableCategories,
            AvailableSubCategories = availableSubCategories
        };

        return model;
    }

    public async  Task UpdateMenuItemAssignmentsAsync(int menuItemId, string[] selectedCategories, string[] selectedSubCategories)
    {
        var menuItem = await _context.MenuItems
            .Include(c => c.MenuItemCategories)
            .Include(c => c.MenuItemSubCategories)
            .FirstOrDefaultAsync(m => m.Id == menuItemId);
        if(menuItem == null)
            throw new KeyNotFoundException();

        var newCategories = selectedCategories
            .Select(cat => new MenuItemCategory() { MenuItemId = menuItemId, CategoryId = cat }).ToList();
        var newSubCategories = selectedSubCategories
            .Select(scat => new MenuItemSubCategory() { MenuItemId = menuItemId, SubCategoryId = scat }).ToList();

        _context.MenuItemCategories.RemoveRange(menuItem.MenuItemCategories);
        _context.MenuItemSubCategories.RemoveRange(menuItem.MenuItemSubCategories);
        _context.MenuItemCategories.AddRange(newCategories);
        _context.MenuItemSubCategories.AddRange(newSubCategories);
        await _context.SaveChangesAsync();
    }
}