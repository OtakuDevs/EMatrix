using EMatrix.DatabaseServices.Admin.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EMatrix.Areas.Admin.Controllers;

[Area("Admin")]
public class InventoryController : Controller
{
    private readonly IManageInventoryService _adminInventoryService;

    public InventoryController(IManageInventoryService adminInventoryService)
    {
        _adminInventoryService = adminInventoryService;
    }

    public async Task<IActionResult> GetAllInventoryItems(int page = 1, string search = "", string category = "", string subCategory = "")
    {
        var model = await _adminInventoryService.GetInventoryIndexAsync(page, search, category, subCategory);
        ViewBag.CurrentPage = model.CurrentPage;
        ViewBag.TotalPages = model.TotalPages;
        return View(model);
    }

    public async Task<IActionResult> GetGroupsTable()
    {
        var model = await _adminInventoryService.GetGroupsIndexAsync();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateGroup(string id, string type, string nameAlias)
    {
        try
        {
            await _adminInventoryService.UpdateGroupAsync(id, type,nameAlias);
        }
        catch (Exception)
        {
            TempData["Error"] = "Неуспешна промяна на публично име. Моля опитайте отново.";
            return RedirectToAction("GetGroupsTable");
        }

        TempData["Success"] = "Успешна промяна на публично име.";
        return RedirectToAction("GetGroupsTable");
    }

    public async Task<JsonResult> GetInventoryItemDetails(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return Json(new { success = false, message = "Невалиден идентификатор." });
        }
        var data = await _adminInventoryService.GetInventoryItemByIdAsync(id);
        if (data == null)
        {
            return Json(new { success = false, message = "Артикулът не е намерен." });
        }
        return Json(new
        {
            success = true,
            data = data
        });
    }

    public async Task<IActionResult> UpdateInventoryItem(string id, string? nameAlias, string? descriptionAlias, float quantity, float price)
    {
        try
        {
            await _adminInventoryService.UpdateInventoryItemAsync(id, nameAlias, descriptionAlias, quantity, price);
        }
        catch (Exception)
        {
            TempData["Error"] = "Възникна грешка при промяната на артикул. Моля опитайте отново.";
            return RedirectToAction("GetAllInventoryItems", "Inventory");
        }

        TempData["Success"] = "Успешна промяна на артикул.";
        return RedirectToAction("GetAllInventoryItems", "Inventory");
    }
}