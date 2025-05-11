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

    public async Task<IActionResult> ProductsPrimaryView(int id = 1, string type = "MenuItem")
    {
        var model = await _productsService.GetPrimaryViewAsync(id, type);
        return View(model);
    }

    public async Task<IActionResult> ProductsSecondaryView(string id, int childId, int optionId, int page = 1)
    {
        ProductsSecondaryViewModel model;

        model = await _productsService.GetSecondaryViewAsync(id, childId, optionId, page);

        ViewBag.CurrentPage = model.CurrentPage;
        ViewBag.TotalPages = model.TotalPages;

        return View(model);
    }
}