namespace EMatrix.ViewModels.Products;

public class ProductsSecondaryViewModel
{
    public AccordionViewModel Accordion { get; set; } = new AccordionViewModel();
    public int OptionId { get; set; }
    public string Title { get; set; } = string.Empty;

    public List<OptionChildViewModel> Options { get; set; } = new List<OptionChildViewModel>();

}

public class OptionChildViewModel
{
    public int Id { get; set; }

    public string Name { get; set; }

    //Subgroup only
    public string? SubGroupId { get; set; }

    //Sets only
    public Dictionary<string, string>? Entries = new();
}
