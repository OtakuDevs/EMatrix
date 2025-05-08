namespace EMatrix.ViewModels.Products;

public class ProductsPrimaryViewModel
{
    public AccordionViewModel Accordion { get; set; } = new AccordionViewModel();

    public MenuPreviewViewModel MenuPreview { get; set; } = new MenuPreviewViewModel();
}