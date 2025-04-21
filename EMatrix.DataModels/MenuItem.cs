using System.ComponentModel.DataAnnotations;

namespace EMatrix.DataModels;

public class MenuItem
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    public int MenuId { get; set; }

    [Required]
    public Menu Menu { get; set; }

    public string Icon { get; set; } = string.Empty;

    public ICollection<MenuItemCategory> MenuItemCategories { get; set; } = new List<MenuItemCategory>();

    public ICollection<MenuItemSubCategory> MenuItemSubCategories { get; set; } = new List<MenuItemSubCategory>();
}