namespace EMatrix.ViewModels.Products;

public class ProductsSecondaryViewModel
{
    public string Title { get; set; }
    public AccordionViewModel Accordion { get; set; } = new AccordionViewModel();

    public List<ProductListingViewModel> Products { get; set; } = new List<ProductListingViewModel>();
}

