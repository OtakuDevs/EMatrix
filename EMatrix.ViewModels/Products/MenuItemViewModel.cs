namespace EMatrix.ViewModels.Products;

public class MenuItemViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public Dictionary<int, string> Options { get; set; } = new Dictionary<int, string>();
}