using EMatrix.ViewModels.Products;

namespace EMatrix.DatabaseServices.Public.Interfaces;

public interface IProductsService
{
    Task<ProductsPrimaryViewModel> GetPrimaryViewAsync(int id, string type);

    Task<ProductsSecondaryViewModel> GetSecondaryViewAsync(string id, int childId, int optionId, int page = 1, string search = "");

    Task<ProductsSearchViewModel> GetSearchViewAsync(int? optionId, string type, string search, int page = 1);

    Task<ProductDetailsViewModel> GetDetailsViewAsync(string id, string type = "MenuItem");
}