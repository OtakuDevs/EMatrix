namespace EMatrix.DataModels;

public class Category
{
    public string Id { get; set; }
    
    public string Name { get; set; }
    
    public string Alias { get; set; }
    
    public ICollection<InventoryItem> InventoryItems { get; set; }
    
    public ICollection<SubCategory> SubCategories { get; set; }
}