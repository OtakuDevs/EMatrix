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
}