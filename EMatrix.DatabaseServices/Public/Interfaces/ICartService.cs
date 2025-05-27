using EMatrix.ViewModels.Cart;

namespace EMatrix.DatabaseServices.Public.Interfaces;

public interface ICartService
{
    Task<CartViewModel> GetCartAsync(List<CartProductViewModel> cart);
}