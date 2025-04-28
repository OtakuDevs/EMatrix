namespace EMatrix.ViewModels.Products;

public class ProductsSecondaryViewModel
{
    public string Title { get; set; } = string.Empty;

    public List<OptionViewModel> Groups { get; set; } = new List<OptionViewModel>();

    public List<OptionViewModel> SubGroups { get; set; } = new List<OptionViewModel>();

    public List<OptionViewModel> Sets { get; set; } = new List<OptionViewModel>();

    public SecondaryViewMode Mode { get; set; }
}

public enum SecondaryViewMode
{
    MenuItem,
    Category,
    SubCategory,
    Set
}

public class OptionViewModel
{
    public string Id { get; set; }

    public string Name { get; set; }

    //Sets only
    public List<string> Entries = new();
}
