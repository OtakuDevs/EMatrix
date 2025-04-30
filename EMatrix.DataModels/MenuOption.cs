namespace EMatrix.DataModels;

public class MenuOption
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    public string Icon { get; set; }

    public int MenuItemId { get; set; }
    public MenuItem MenuItem { get; set; } = null!;

    public ICollection<MenuOptionChild> Children { get; set; } = new List<MenuOptionChild>();
}
