using EMatrix.DatabaseServices.Public.Interfaces;
using EMatrix.ViewModels.Products;
using Microsoft.AspNetCore.Mvc;
// ReSharper disable ConvertToPrimaryConstructor

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
        try
        {
            var model = await _productsService.GetPrimaryViewAsync(id, type);
            return View(model);
        }
        catch (Exception e)
        {
            return RedirectToAction("Error", "Home", new { statusCode = 500, e.Message });
        }
    }

    public async Task<IActionResult> ProductsSecondaryView(string id, int childId, int optionId, int page = 1, string search = "")
    {
        try
        {
            var model = await _productsService.GetSecondaryViewAsync(id, childId, optionId, page, search);

            ViewBag.CurrentPage = model.CurrentPage;
            ViewBag.TotalPages = model.TotalPages;

            return View(model);
        }
        catch (Exception e)
        {
            return RedirectToAction("Error", "Home", new { statusCode = 500, e.Message });
        }

    }

    public async Task<IActionResult> ProductsSearchView(int? optionId, string type, string search = "", int page = 1)
    {
        try
        {
            var model = await _productsService.GetSearchViewAsync(optionId, type, search, page);

            ViewBag.CurrentPage = model.CurrentPage;
            ViewBag.TotalPages = model.TotalPages;
            return View(model);
        }
        catch (Exception e)
        {
            return RedirectToAction("Error", "Home", new { statusCode = 404, e.Message });
        }

    }
}