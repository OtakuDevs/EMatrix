namespace EMatrix.DataModels;

public class MenuOptionChild
{
    public int Id { get; set; }

    public int Order { get; set; }

    public int MenuOptionId { get; set; }

    public MenuOption MenuOption { get; set; } = null!;

    //if subgroup
    public string? SubGroupId { get; set; }
    public SubCategory? SubGroup { get; set; }

    //if subgroup set
    public string? DisplayName { get; set; } // e.g. "Диоди ценерови"

    public int? SubGroupSetId { get; set; }
    public SubGroupSet? SubGroupSet { get; set; }
}
