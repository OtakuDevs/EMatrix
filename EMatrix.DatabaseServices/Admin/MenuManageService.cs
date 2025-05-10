using System.Text.Json;
using EMatrix.Constants;
using EMatrix.Database;
using EMatrix.DatabaseServices.Admin.Interfaces;
using EMatrix.DataModels;
using EMatrix.DataModels.DTOs;
using EMatrix.ViewModels.Admin;
using EMatrix.ViewModels.MenuEditor;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
// ReSharper disable ConvertToPrimaryConstructor

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
            .Include(r => r.MenuItems)
            .ThenInclude(o => o.Options)
            .ThenInclude(c => c.Children)
            .FirstOrDefaultAsync(m => m.Id == id);
        if(menu == null)
            throw new KeyNotFoundException();

        var model = new MenuAdminViewModel()
        {
            MenuItems = menu.MenuItems.OrderBy(r => r.Name).Select(r => new MenuItemRowModel()
            {
                Id = r.Id,
                Name = r.Name,
                Position = r.Order,
                Options = r.Options.SelectMany(o => o.Children).Select(c => new SelectListItem()
                {
                    Value = c.Id.ToString(),
                    Text = c.DisplayName
                }).ToList(),
            })
            .ToList(),
        };
        return model;
    }

    public async Task<bool> AddMenuItemAsync(string name, int position)
    {
        try
        {
            var menuItem = new MenuItem()
            {
                Name = name,
                Order = position,
            };
            _context.MenuItems.Add(menuItem);
            await _context.SaveChangesAsync();
        }
        catch (Exception)
        {
            return false;
        }
        return true;
    }

    public async Task RenameMenuItemAsync(int id, string? name, int position = 0)
    {
        var menuItem = _context.MenuItems.FirstOrDefault(m => m.Id == id);
        if(menuItem == null)
            throw new KeyNotFoundException();
        menuItem.Name = string.IsNullOrEmpty(name) ? menuItem.Name : name;
        menuItem.Order = position > 0 ? position : menuItem.Order;
        _context.MenuItems.Update(menuItem);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteMenuItemAsync(int id)
    {
        var menuItem = _context.MenuItems.FirstOrDefault(m => m.Id == id);
        if(menuItem == null)
            throw new KeyNotFoundException();
        if (!string.IsNullOrEmpty(menuItem.Icon))
        {
            // Convert relative path to absolute path
            var iconPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", menuItem.Icon.TrimStart('/'));

            if (File.Exists(iconPath))
            {
                File.Delete(iconPath);
            }

            // Clear the icon field
            menuItem.Icon = string.Empty;
        }
        _context.MenuItems.Remove(menuItem);
        await _context.SaveChangesAsync();
    }

   public async Task<MenuItemAdminViewModel> GetMenuItemModelAsync(int id)
{
    var menuItem = await GetMenuItem(id);

    if (menuItem == null)
        throw new KeyNotFoundException();

    // Build model
    var model = new MenuItemAdminViewModel
    {
        Id = menuItem.Id,
        Name = menuItem.Name,
        Options = menuItem.Options.Select(option => new MenuOptionAdminViewModel
        {
            Id = option.Id,
            Name = option.Name,
            Order = option.Order,
            Children = option.Children.Select(child =>
            {
                if (child.SubGroup != null)
                {
                    return new MenuOptionChildAdminViewModel
                    {
                        Id = child.Id,
                        DisplayName = child.SubGroup.Alias ?? child.DisplayName,
                        Type = "SubGroup",
                        ReferenceId = child.SubGroupId
                    };
                }
                else if (child.SubGroupSet != null)
                {
                    return new MenuOptionChildAdminViewModel
                    {
                        Id = child.Id,
                        DisplayName = child.DisplayName ?? child.SubGroupSet.Name,
                        Type = "SubGroupSet",
                        ReferenceId = child.SubGroupSetId.ToString(),
                        SubGroupSetItems = child.SubGroupSet.Items
                            .Select(i => new SubGroupBasicViewModel
                            {
                                Id = i.SubGroup.Id,
                                Alias = i.SubGroup.Alias
                            }).ToList()
                    };
                }

                return null!;
            }).Where(c => c != null).ToList()
        }).ToList(),

        // Optional: populate for creation UI
        AvailableCategories = await _context.Categories
            .OrderBy(c => c.Id)
            .Select(c => new SelectListItem
            {
                Text = c.Alias,
                Value = c.Id.ToString()
            })
            .ToListAsync(),

        AvailableSubCategories = await _context.SubCategories
            .OrderBy(sc => sc.Id)
            .Select(sc => new SelectListItem
            {
                Text = sc.Alias,
                Value = sc.Id.ToString()
            })
            .ToListAsync()
    };

    return model;
}



public async Task UpdateMenuItemAssignmentsAsync(MenuItemAdminViewModel model)
    {
        var menuItem = await GetMenuItem(model.Id);
        if(menuItem == null)
            throw new KeyNotFoundException();
        _context.MenuOptionChildren.RemoveRange(menuItem.Options.SelectMany(o => o.Children));
        _context.MenuOptions.RemoveRange(menuItem.Options);
        menuItem.Options.Clear();

        //Create the options
        foreach (var menuOption in model.Options)
        {
            var option = new MenuOption()
            {
                Order = menuOption.Order,
                Name = menuOption.Name,
                Icon = "to be implemented",
                MenuItemId = menuItem.Id,
                Children = new List<MenuOptionChild>(),
            };
            foreach (var child in menuOption.Children)
            {
                var optionChild = new MenuOptionChild();

                switch (child.Type)
                {
                    case "SubGroup":
                        optionChild.SubGroupId = child.ReferenceId;
                        optionChild.DisplayName = child.DisplayName;
                        break;

                    case "SubGroupSet":
                        if (child.SubGroupSetItems != null && child.SubGroupSetItems.Any())
                        {
                            var subGroupSet = new SubGroupSet
                            {
                                Name = child.DisplayName,
                                Items = child.SubGroupSetItems
                                    .Select(item => new SubGroupSetItem
                                    {
                                        SubGroupId = item.Id,
                                    }).ToList()
                            };
                            optionChild.DisplayName = subGroupSet.Name;
                            optionChild.SubGroupSet = subGroupSet;
                        }
                        break;
                }

                option.Children.Add(optionChild);
            }
            menuItem.Options.Add(option);
        }
        _context.MenuOptions.AddRange(menuItem.Options);

        await _context.SaveChangesAsync();
    }


    public async Task UpdateItemImageAsync(int menuItemId, int? selectedOption, IFormCollection form)
{
    if (form.Files.Count == 0 || form.Files["imageFile"] == null)
        throw new KeyNotFoundException("Моля изберете файл.");

    var file = form.Files["imageFile"]!;
    var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

    if (fileExtension != ".jpg" && fileExtension != ".jpeg" && fileExtension != ".png")
        throw new ArgumentException("Само файлове с разширение .jpg, .jpeg, .png са разрешени!");

    string? oldIconPath = null;
    string? newIconRelativePath = null;

    if (selectedOption.HasValue)
    {
        var option = await _context.MenuOptionChildren.FirstOrDefaultAsync(o => o.Id == selectedOption);
        if (option == null)
            throw new KeyNotFoundException("Опцията не е открита.");

        if (!string.IsNullOrEmpty(option.Icon))
        {
            oldIconPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", option.Icon.TrimStart('/'));
        }

        var fileName = Path.GetFileName(file.FileName);
        newIconRelativePath = Path.Combine("/images/menu-categories", fileName);
        var newIconFullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", newIconRelativePath.TrimStart('/'));

        await using (var stream = new FileStream(newIconFullPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        option.Icon = newIconRelativePath;
        _context.MenuOptionChildren.Update(option);
    }
    else
    {
        var menuItem = await _context.MenuItems.FirstOrDefaultAsync(m => m.Id == menuItemId);
        if (menuItem == null)
            throw new KeyNotFoundException("Категорията не е открита.");

        if (!string.IsNullOrEmpty(menuItem.Icon))
        {
            oldIconPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", menuItem.Icon.TrimStart('/'));
        }

        var fileName = Path.GetFileName(file.FileName);
        newIconRelativePath = Path.Combine("/images/menu-categories", fileName);
        var newIconFullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", newIconRelativePath.TrimStart('/'));

        await using (var stream = new FileStream(newIconFullPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        menuItem.Icon = newIconRelativePath;
        _context.MenuItems.Update(menuItem);
    }

    if (oldIconPath != null && File.Exists(oldIconPath))
    {
        File.Delete(oldIconPath);
    }

    await _context.SaveChangesAsync();
}


    private async Task<MenuItem?> GetMenuItem(int id)
    {
        var menuItem = await _context.MenuItems
            .Include(m => m.Options)
            .ThenInclude(o => o.Children)
            .ThenInclude(c => c.SubGroup)
            .Include(m => m.Options)
            .ThenInclude(o => o.Children)
            .ThenInclude(c => c.SubGroupSet)
            .ThenInclude(s => s.Items)
            .ThenInclude(i => i.SubGroup)
            .FirstOrDefaultAsync(m => m.Id == id);
        return menuItem;
    }

}
