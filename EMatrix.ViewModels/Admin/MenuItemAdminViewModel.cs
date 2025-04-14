using Microsoft.AspNetCore.Mvc.Rendering;

namespace EMatrix.ViewModels.Admin;

public class MenuItemAdminViewModel
{
    public int Id { get; set; }

    public string Name { get; set; }

    public Dictionary<string, string> Categories { get; set; } = new Dictionary<string, string>();

    public Dictionary<string, string> SubCategories { get; set; } = new Dictionary<string, string>();

    public List<SelectListItem> AvailableCategories { get; set; } = new List<SelectListItem>();

    public List<SelectListItem> AvailableSubCategories { get; set; } = new List<SelectListItem>();
}