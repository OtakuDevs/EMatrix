using System.ComponentModel.DataAnnotations;

namespace EMatrix.DataModels;

public class InventoryItem
{
    public string Id { get; set; }
    
    public string CategoryId { get; set; }
    
    [Required]
    public Category Category { get; set; }
    
    public string SubCategoryId { get; set; }
    
    [Required]
    public SubCategory SubCategory { get; set; }
    
    [Required]
    public string Name { get; set; }

    [Required]
    public string NameAlias { get; set; }
    
    [Required]
    public string Description { get; set; }

    [Required]
    public string DescriptionAlias { get; set; }

    [Required]
    public string Unit { get; set; }
    
    public float Price { get; set; }
    
    public float Quantity { get; set; }
}