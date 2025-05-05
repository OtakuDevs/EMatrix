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
    public async Task<IActionResult> ProductsPrimaryView()
    {
        var model = await _productsService.GetPrimaryViewAsync(1);
        return View(model);
    }

    public async Task<IActionResult> ProductsSecondaryView(int? menuItemId, int? optionId)
    {
        ProductsSecondaryViewModel model;

        if (menuItemId.HasValue)
        {
            model = await _productsService.GetSecondaryViewByMenuItemId(menuItemId.Value);
        }
        else if (optionId.HasValue)
        {
            model = await _productsService.GetSecondaryViewByOptionId(optionId.Value);
        }

        else
        {
            return NotFound();
        }

        return View(model);
    }
}