namespace EMatrix.ViewModels.Admin;

public class InventoryIndexViewModel
{
    public ICollection<InventoryItemViewModel> InventoryItems { get; set; } = new List<InventoryItemViewModel>();

    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }

    public string SearchTerm { get; set; }
}