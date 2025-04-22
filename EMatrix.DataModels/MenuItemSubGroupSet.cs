namespace EMatrix.DataModels;

public class MenuItemSubGroupSet
{
    public int Id { get; set; }

    public string Name { get; set; } // "ДИОДИ ЦЕНЕРОВИ"

    public int MenuItemId { get; set; }

    public MenuItem MenuItem { get; set; }

    public ICollection<MenuItemSubGroupSetEntry> Entries { get; set; } = new List<MenuItemSubGroupSetEntry>();
}