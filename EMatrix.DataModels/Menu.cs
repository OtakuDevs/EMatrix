using System.ComponentModel.DataAnnotations;

namespace EMatrix.DataModels;

public class Menu
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public List<MenuItem> MenuItems { get; set; } = new();
}
