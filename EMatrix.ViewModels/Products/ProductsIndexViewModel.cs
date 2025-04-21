namespace EMatrix.ViewModels.Products;

public class ProductsIndexViewModel
{
    public List<MenuItemViewModel> Menu { get; set; } = new List<MenuItemViewModel>();

    public List<MenuItemPreviewModel> MenuItemsGrid { get; set; } = new List<MenuItemPreviewModel>();
}