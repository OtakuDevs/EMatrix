using EMatrix.ViewModels.Admin;

namespace EMatrix.DatabaseServices.Admin.Interfaces;

public interface IMenuManageService
{
    Task<MenuAdminViewModel> GetMenu(int id);

    Task<bool> AddMenuItemAsync(string name);

    Task RenameMenuItemAsync(int id, string name);

    Task DeleteMenuItemAsync(int id);

    Task<MenuItemAdminViewModel> GetMenuItemModelAsync(int id);

    Task UpdateMenuItemAssignmentsAsync(int menuItemId, string[] selectedCategories, string[] selectedSubCategories);
}