using EMatrix.ViewModels.Products;

namespace EMatrix.DatabaseServices.Public.Interfaces;

public interface IProductsService
{
    Task<ProductsPrimaryViewModel> GetPrimaryViewAsync(int id);

    Task<ProductsSecondaryViewModel> GetSecondaryViewByMenuItemId(int id);

    Task<ProductsSecondaryViewModel> GetSecondaryViewByOptionId(int optionId);
}