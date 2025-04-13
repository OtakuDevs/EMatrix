using System.ComponentModel.DataAnnotations;

namespace EMatrix.DataModels;

public class Menu
{
    public int Id { get; set; }

    public List<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
}