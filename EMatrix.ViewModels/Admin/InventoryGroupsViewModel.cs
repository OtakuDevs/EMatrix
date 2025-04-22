using System.Collections;

namespace EMatrix.ViewModels.Admin;

public class InventoryGroupsViewModel
{
    public ICollection<GroupViewModel> Groups { get; set; } = new List<GroupViewModel>();
}

public class SubGroupViewModel
{
    public string Id { get; set; }

    public string Name { get; set; }

    public string Alias { get; set; }
}

public class GroupViewModel : SubGroupViewModel
{
    public ICollection<SubGroupViewModel> SubGroups = new List<SubGroupViewModel>();
}