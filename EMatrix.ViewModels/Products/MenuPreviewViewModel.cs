namespace EMatrix.ViewModels.Products;

public class MenuPreviewViewModel
{
    public int Id { get; set; }
    public string Type { get; set; }

    public List<MenuOptionViewModel> Options = new List<MenuOptionViewModel>();
}