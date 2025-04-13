using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMatrix.DataModels;

public class MenuItemSubCategory
{
    [ForeignKey(nameof(MenuItem))]
    public int MenuItemId { get; set; }

    [Required]
    public MenuItem MenuItem { get; set; } = null!;

    [ForeignKey(nameof(SubCategory))]
    public string SubCategoryId { get; set; }

    [Required]
    public SubCategory SubCategory { get; set; } = null!;
}