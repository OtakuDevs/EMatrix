namespace EMatrix.ViewModels.Products;

public class MenuItemOptionViewModel
{
    public string Name { get; set; } = null!;

    public string Icon { get; set; } = string.Empty;

    public Dictionary<string, string> Options { get; set; } = new Dictionary<string, string>();
}