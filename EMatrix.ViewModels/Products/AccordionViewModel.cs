namespace EMatrix.ViewModels.Products;

public class AccordionViewModel
{
    public string Type { get; set; } = null!;

    public string SelectedSubGroup{ get; set; }
    public List<AccordionOptionViewModel> Options { get; set; } = new List<AccordionOptionViewModel>();
}