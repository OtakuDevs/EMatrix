using EMatrix.ViewModels.Products;

namespace EMatrix.DatabaseServices.Public.Interfaces;

public interface IProductsService
{
    Task<ProductsPrimaryViewModel> GetPrimaryViewAsync(int id, string type);

    Task<ProductsSecondaryViewModel> GetSecondaryViewAsync(string id, int childId, int optionId);
}