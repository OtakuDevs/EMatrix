namespace EMatrix.ViewModels.Admin;

public class MenuItemAdminViewModel
{
    public int Id { get; set; }

    public string Name { get; set; }

    public Dictionary<string, string> Categories { get; set; } = new Dictionary<string, string>();

    public Dictionary<string, string> SubCategories { get; set; } = new Dictionary<string, string>();
}