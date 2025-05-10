namespace EMatrix.DataModels;

public class MenuOptionChild
{
    public int Id { get; set; }

    public string Icon { get; set; } = string.Empty;

    public string? DisplayName { get; set; }

    public int MenuOptionId { get; set; }

    public MenuOption MenuOption { get; set; } = null!;

    //if subgroup
    public string? SubGroupId { get; set; }
    public SubCategory? SubGroup { get; set; }

    //if subgroup set
    public int? SubGroupSetId { get; set; }
    public SubGroupSet? SubGroupSet { get; set; }
}
