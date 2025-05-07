namespace EMatrix.ViewModels.Products;

public class AccordionOptionViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public List<AccordionOptionChildViewModel> Options { get; set; } =new List<AccordionOptionChildViewModel>();
}

public class AccordionOptionChildViewModel
{
    public int Id { get; set; }

    public string Name { get; set; }

    //Subgroup only
    public string? SubGroupId { get; set; }

    //Sets only
    public Dictionary<string, string>? Entries = new();
}