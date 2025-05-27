using EMatrix.DatabaseServices.Public.Interfaces;
using EMatrix.ViewModels.Cart;

namespace EMatrix.DatabaseServices.Public;

public class CartService : ICartService
{
    public Task<CartViewModel> GetCartAsync(List<CartProductViewModel> cart)
    {
        var model = new CartViewModel
        {
            Products = cart.Select(s => new CartProductViewModel()
            {
                Id = s.Id,
                Name = s.Name,
                Icon = s.Icon ?? "/images/default/placeholder.png",
                Subgroup = s.Subgroup,
                Price = s.Price,
                Qty = s.Qty,
            }).ToList()
        };

        return Task.FromResult(model);
    }
}