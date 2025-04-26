using EMatrix.ViewModels.Products;

namespace EMatrix.DatabaseServices.Public.Interfaces;

public interface IProductsService
{
    Task<ProductsIndexViewModel> GetProductsAsync(int id);

    Task<MenuItemOptionViewModel> GetMenuItemResultAsync(string? categoryId, string? subCategoryId, int subGroupSetId = 0);
}