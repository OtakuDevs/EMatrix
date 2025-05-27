namespace EMatrix.ViewModels.Cart;

public class CartProductViewModel
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Subgroup { get; set; }
    public string Icon { get; set; }
    public decimal Price { get; set; }
    public int Qty { get; set; }
}