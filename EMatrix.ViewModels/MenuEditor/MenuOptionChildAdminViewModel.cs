namespace EMatrix.ViewModels.MenuEditor;

public class MenuOptionChildAdminViewModel
{
    public int Id { get; set; }
    public string DisplayName { get; set; } = null!;
    public string Type { get; set; } = null!;
    public string? ReferenceId { get; set; }

    public List<SubGroupBasicViewModel>? SubGroupSetItems { get; set; } // only for SubGroupSet
}