using EMatrix.ViewModels.Products;

namespace EMatrix.DatabaseServices.Public.Interfaces;

public interface IProductsService
{
    Task<ProductsPrimaryViewModel> GetPrimaryViewAsync(int id);

    Task<ProductsSecondaryViewModel> GetSecondaryViewByMenuItemId(int id);

    Task<ProductsSecondaryViewModel> GetSecondaryViewByCategoryId(string id);

    Task<ProductsSecondaryViewModel> GetSecondaryViewBySubCategoryId(string id);

    Task<ProductsSecondaryViewModel> GetSecondaryViewBySubGroupSetId(int id);
}