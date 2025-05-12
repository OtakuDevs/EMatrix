namespace EMatrix.ViewModels.Products;

public class ProductsSearchViewModel
{
    public AccordionViewModel Accordion { get; set; } = new AccordionViewModel();

    public List<ProductListingViewModel> Products { get; set; } = new List<ProductListingViewModel>();

    public int CurrentPage { get; set; }

    public int TotalPages { get; set; }

    public string SearchTerm { get; set; }
}