namespace EMatrix.ViewModels.Products;

public class MenuItemPreviewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Icon { get; set; } = null!;

    public ICollection<string> Items { get; set; } = new List<string>();
}