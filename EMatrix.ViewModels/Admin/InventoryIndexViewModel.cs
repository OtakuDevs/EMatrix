using Microsoft.AspNetCore.Mvc.Rendering;

namespace EMatrix.ViewModels.Admin;

public class InventoryIndexViewModel
{
    public ICollection<InventoryItemViewModel> InventoryItems { get; set; } = new List<InventoryItemViewModel>();

    public List<SelectListItem> AvailableCategories { get; set; } = new List<SelectListItem>();

    public List<SelectListItem> AvailableSubCategories { get; set; } = new List<SelectListItem>();

    public string CategoryFilter { get; set; }

    public string SubCategoryFilter { get; set; }

    public int CurrentPage { get; set; }

    public int TotalPages { get; set; }

    public string SearchTerm { get; set; }
}
