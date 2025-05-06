namespace EMatrix.ViewModels.Products;

public class ProductsSecondaryViewModel
{
    public string Title { get; set; } = string.Empty;

    public List<OptionViewModel> Options { get; set; } = new List<OptionViewModel>();

}

public class OptionViewModel
{
    public int Id { get; set; }

    public string Name { get; set; }

    //Subgroup only
    public string? SubGroupId { get; set; }

    //Sets only
    public Dictionary<string, string>? Entries = new();
}
