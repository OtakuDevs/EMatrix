namespace EMatrix.ViewModels.MenuEditor;

public class MenuOptionAdminViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public int Order { get; set; }

    public List<MenuOptionChildAdminViewModel> Children { get; set; } = new();
}