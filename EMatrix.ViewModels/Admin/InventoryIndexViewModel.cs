namespace EMatrix.ViewModels.Admin;

public class InventoryIndexViewModel
{
    public ICollection<InventoryItemViewModel> InventoryItems { get; set; } = new List<InventoryItemViewModel>();
}