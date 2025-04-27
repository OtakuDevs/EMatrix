using EMatrix.DatabaseServices.Public.Interfaces;
using EMatrix.ViewModels.Products;
using Microsoft.AspNetCore.Mvc;

namespace EMatrix.Controllers;

public class ProductsController : Controller
{
    private readonly IProductsService _productsService;

    public ProductsController(IProductsService productsService)
    {
        _productsService = productsService;
    }

    // GET
    public async Task<IActionResult> Index()
    {
        var model = await _productsService.GetPrimaryViewAsync(1);
        return View(model);
    }

    public async Task<IActionResult> GetMenuItemResult(string? categoryId, string? subCategoryId, int? subGroupSetId, int? menuItemId)
    {
        ProductsSecondaryViewModel model;

        if (menuItemId.HasValue)
        {
            model = await _productsService.GetSecondaryViewByMenuItemId(menuItemId.Value);
        }
        else if (!string.IsNullOrEmpty(categoryId))
        {
            model = await _productsService.GetSecondaryViewByCategoryId(categoryId);
        }
        else if (!string.IsNullOrEmpty(subCategoryId))
        {
            model = await _productsService.GetSecondaryViewBySubCategoryId(subCategoryId);
        }
        else if (subGroupSetId.HasValue)
        {
            model = await _productsService.GetSecondaryViewBySubGroupSetId(subGroupSetId.Value);
        }
        else
        {
            return NotFound();
        }

        return View(model);
    }
}