using Microsoft.AspNetCore.Mvc.Rendering;

namespace EMatrix.ViewModels.MenuEditor;

public class MenuAdminViewModel
{
    public List<MenuItemRowModel> MenuItems { get; set; } = new List<MenuItemRowModel>();
}

public class MenuItemRowModel
{
    public int Id { get; set; }

    public string Name { get; set; }

    public int Position { get; set; }

    public List<SelectListItem> Options { get; set; } = new List<SelectListItem>();

}