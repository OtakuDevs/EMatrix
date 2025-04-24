using EMatrix.ViewModels.Admin;
using Microsoft.AspNetCore.Http;

namespace EMatrix.DatabaseServices.Admin.Interfaces;

public interface IMenuManageService
{
    Task<MenuAdminViewModel> GetMenu(int id);

    Task<bool> AddMenuItemAsync(string name, int position);

    Task RenameMenuItemAsync(int id, string? name, int position = 0);

    Task DeleteMenuItemAsync(int id);

    Task<MenuItemAdminViewModel> GetMenuItemModelAsync(int id);

    Task UpdateMenuItemAssignmentsAsync(int menuItemId, string[] selectedCategories, string[] selectedSubCategories, string groupedSubCategoriesJson);

    Task UpdateMenuItemImageAsync(int menuItemId, IFormCollection form);
}