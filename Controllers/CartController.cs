using EMatrix.DatabaseServices.Public.Interfaces;
using EMatrix.ViewModels.Cart;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EMatrix.Controllers;

public class CartController : Controller
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [HttpPost]
    public async Task<IActionResult> ReceiveCartContent([FromBody] List<CartProductViewModel> cart)
    {
        var model = await _cartService.GetCartAsync(cart);
        TempData["CartModel"] = JsonConvert.SerializeObject(model); // Store in TempData
        return Ok();
    }

    [HttpGet]
    public IActionResult CartPreview()
    {
        if (TempData["CartModel"] is string cartJson)
        {
            var model = JsonConvert.DeserializeObject<CartViewModel>(cartJson);
            return View("CartPreview", model);
        }

        return RedirectToAction("ProductsPrimaryView", "Products"); // fallback
    }


}