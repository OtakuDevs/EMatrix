namespace EMatrix.ViewModels.Products;

public class ProductListingViewModel
{
    public string Id { get; set; } = null!;

    public string SubCategory { get; set; } = null!;

    public string NameAlias  { get; set; } = null!;

    public string? DescriptionAlias  { get; set; }

    public string Icon { get; set; } = null!;

    public float Price { get; set; }

    public bool Availability { get; set; }

}