namespace EMatrix.DataModels;

public class MenuItemSubGroupSetEntry
{
    public int Id { get; set; }

    public string SubCategoryId { get; set; }

    public string SubCategoryName { get; set; } // optional for caching

    public int MenuItemSubGroupSetId { get; set; }

    public MenuItemSubGroupSet MenuItemSubGroupSet { get; set; }
}