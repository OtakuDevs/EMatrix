using System.ComponentModel.DataAnnotations;

namespace EMatrix.DataModels;

public class MenuItem
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    public string Icon { get; set; } = null!;
    public int Order { get; set; }

    public int MenuId { get; set; }

    public Menu Menu { get; set; } = null!;

    public ICollection<MenuOption> Options { get; set; } = new List<MenuOption>();
}

