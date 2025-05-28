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
        HttpContext.Session.SetString("CartModel", JsonConvert.SerializeObject(model));
        return Ok();
    }

    [HttpGet]
    public IActionResult CartPreview()
    {
        var cartJson = HttpContext.Session.GetString("CartModel");
        if (!string.IsNullOrEmpty(cartJson))
        {
            var model = JsonConvert.DeserializeObject<CartViewModel>(cartJson);
            return View("CartPreview", model);
        }

        return RedirectToAction("ProductsPrimaryView", "Products"); // fallback
    }


}