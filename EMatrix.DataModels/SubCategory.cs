using System.ComponentModel.DataAnnotations;

namespace EMatrix.DataModels;

public class SubCategory
{
    public string Id { get; set; }
    
    public string Name { get; set; }
    
    public string Alias { get; set; }
    
    public string CategoryId { get; set; }
    
    [Required]
    public Category Category { get; set; }
    
    public ICollection<InventoryItem> InventoryItems { get; set; }

}