using Microsoft.AspNetCore.Mvc.Rendering;

namespace EMatrix.ViewModels.MenuEditor;

public class MenuItemAdminViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public List<MenuOptionAdminViewModel> Options { get; set; } = new();

    public List<SelectListItem> AvailableCategories { get; set; } = new();

    public List<SelectListItem> AvailableSubCategories { get; set; } = new();
}
