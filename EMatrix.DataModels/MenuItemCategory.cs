using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMatrix.DataModels;

public class MenuItemCategory
{
    [ForeignKey(nameof(MenuItem))]
    public int MenuItemId { get; set; }

    [Required]
    public MenuItem MenuItem { get; set; } = null!;

    [ForeignKey(nameof(Category))]
    public string CategoryId { get; set; }

    [Required]
    public Category Category { get; set; } = null!;
}