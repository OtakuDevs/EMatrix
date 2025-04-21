namespace EMatrix.ViewModels.Products;

public class MenuItemViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public Dictionary<string, string> Categories { get; set; } = new Dictionary<string, string>();

    public Dictionary<string, string> SubCategories { get; set; } = new Dictionary<string, string>();

}