namespace EMatrix.ViewModels.Products;

public class AccordionViewModel
{
    public string Type { get; set; } = null!;

    //Same level as the selected subgroup but the actual menu option child id
    //to be able to carry the parameter across views
    public int SelectedChildId { get; set; }

    // subgroup id when making query to db to extract items
    // (cause the items in db has subgroups - no relation to the menu option children )
    // or to set nav links as active
    public string SelectedSubGroupId{ get; set; }
    
    public string SelectedSubGroupName { get; set; }
    public List<AccordionOptionViewModel> Options { get; set; } = new List<AccordionOptionViewModel>();
}