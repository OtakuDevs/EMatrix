namespace EMatrix.DataModels;

public class SubGroupSet
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public ICollection<SubGroupSetItem> Items { get; set; } = new List<SubGroupSetItem>();
}