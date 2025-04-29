namespace EMatrix.DataModels;

public class SubGroupSetItem
{
    public int Id { get; set; }
    public int SubGroupSetId { get; set; }
    public SubGroupSet SubGroupSet { get; set; } = null!;

    public string SubGroupId { get; set; }
    public SubCategory SubGroup { get; set; } = null!;
}