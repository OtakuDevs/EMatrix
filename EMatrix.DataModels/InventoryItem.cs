using System.ComponentModel.DataAnnotations;

namespace EMatrix.DataModels;

public class InventoryItem
{
    public int Id { get; set; }
    
    public int CategoryId { get; set; }
    
    [Required]
    public Category Category { get; set; }
    
    public int SubCategoryId { get; set; }
    
    [Required]
    public SubCategory SubCategory { get; set; }
    
    [Required]
    public string Name { get; set; }
    
    [Required]
    public string Description { get; set; }

    [Required]
    public string Unit { get; set; }
    
    public float Price { get; set; }
    
    public float Quantity { get; set; }
}