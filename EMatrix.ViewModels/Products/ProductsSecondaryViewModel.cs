namespace EMatrix.ViewModels.Products;

public class ProductsSecondaryViewModel
{
    public string Title { get; set; } = string.Empty;

    public Dictionary<string, string> Categories { get; set; } = new();

    public Dictionary<string, string> SubCategories { get; set; } = new();

    public Dictionary<string, string> Sets{ get; set; } = new();

    public SecondaryViewMode Mode { get; set; }
}

public enum SecondaryViewMode
{
    MenuItem,
    Category,
    SubCategory,
    Set
}
