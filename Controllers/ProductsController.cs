using EMatrix.DatabaseServices.Public.Interfaces;
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
        var model = await _productsService.GetProductsAsync(1);
        return View(model);
    }

    public async Task<IActionResult> GetMenuItemResult(string? categoryId, string? subCategoryId, int subGroupSetId = 0)
    {
        var model = await _productsService.GetMenuItemResultAsync(categoryId, subCategoryId, subGroupSetId);
        return View(model);
    }
}