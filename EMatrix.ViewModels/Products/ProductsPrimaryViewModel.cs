namespace EMatrix.ViewModels.Products;

public class ProductsPrimaryViewModel
{
    public AccordionViewModel Accordion { get; set; } = new AccordionViewModel();

    public List<MenuItemPreviewModel> MenuItemsGrid { get; set; } = new List<MenuItemPreviewModel>();
}