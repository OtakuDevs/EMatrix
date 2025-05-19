namespace EMatrix.ViewModels.Products;

public class ProductDetailsViewModel
{
    public string Id { get; set; }

    public string Name { get; set; }

    public Dictionary<string, string> Description { get; set; }

    public string Icon { get; set; }

    public float Price { get; set; }

    public bool Availability { get; set; }

    public string Unit { get; set; }

    public OptionModel ProductMenuOption { get; set; } = new OptionModel();

    public OptionModel ProductMenuOptionChild { get; set; } = new OptionModel();

    public AccordionViewModel Accordion { get; set; } = new AccordionViewModel();
}

public class OptionModel()
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? SubGroupId { get; set; }
}