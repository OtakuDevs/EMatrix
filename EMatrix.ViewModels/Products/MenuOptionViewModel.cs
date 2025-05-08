namespace EMatrix.ViewModels.Products;

public class MenuOptionViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Icon { get; set; } = null!;

    public Dictionary<string, string> Children { get; set; } = new Dictionary<string,string>();
}
