using EMatrix.DatabaseServices.Admin.Interfaces;
using Microsoft.AspNetCore.Mvc;
using EMatrix.Constants;
using EMatrix.ViewModels.MenuEditor;
using EMatrix.ViewModels.Products;

namespace EMatrix.Areas.Admin.Controllers;

[Area("Admin")]
public class MenuEditorController : Controller
{
    private readonly IMenuManageService _menuManageService;

    public MenuEditorController(IMenuManageService menuManageService)
    {
        _menuManageService = menuManageService;
    }
    public async Task<IActionResult> GetMenuManagement()
    {
        try
        {
            var model = await _menuManageService.GetMenu(ConfigurationConstants.MenuId);
            return View(model);
        }
        catch (Exception )
        {
            TempData["Error"] = StatusMessages.MenuEditorFailedToLoad;
            return RedirectToAction("Index", "AdminPanel");
        }

    }

    [HttpPost]
    public async Task<IActionResult> AddMenuItem(string name, int position)
    {
        var result = await _menuManageService.AddMenuItemAsync(name, position);
        if (!result)
            TempData["Error"] = $"Неуспешно добавяне на категория: {name}";
        else
            TempData["Success"] = $"Успешно добавяне на категория: {name}";

        return RedirectToAction("GetMenuManagement");
    }

    [HttpPost]
    public async Task<IActionResult> RenameMenuItem(int id, string? name, int position = 0)
    {
        try
        {
            await _menuManageService.RenameMenuItemAsync(id, name, position);
        }
        catch (Exception)
        {
            TempData["Error"] = "Неуспешно промяна име / позиция на категория.";
            return RedirectToAction("GetMenuManagement");
        }

        TempData["Success"] = "Успешно промяна име / позиция на категория.";
        return RedirectToAction("GetMenuManagement");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteMenuItem(int id)
    {
        try
        {
            await _menuManageService.DeleteMenuItemAsync(id);
        }
        catch (Exception)
        {
            TempData["Error"] = "Неуспешно изтриване на категория.";
            return RedirectToAction("GetMenuManagement");
        }

        TempData["Success"] = "Успешно изтриване на категория.";
        return RedirectToAction("GetMenuManagement");
    }

    [HttpGet]
    public async Task<IActionResult> UpdateMenuItemCategories(int menuItemId)
    {
        try
        {
            var model = await _menuManageService.GetMenuItemModelAsync(menuItemId);
            return View(model);
        }
        catch (Exception)
        {
            TempData["Error"] = StatusMessages.MenuItemEditorFailedToLoad;
            return RedirectToAction("Index", "AdminPanel");
        }
    }

    [HttpPost]
    public async Task<IActionResult> UpdateMenuItemAssignments(MenuItemAdminViewModel model)
    {
        try
        {
            await _menuManageService.UpdateMenuItemAssignmentsAsync(model);
        }
        catch (Exception)
        {
            TempData["Error"] = "Неуспешна промяна на групи и подгрупи за дадената категория.";
            return RedirectToAction("GetMenuManagement");
        }
        TempData["Success"] = "Успешна промяна на групи и подгрупи за дадената категория.";
        return RedirectToAction("GetMenuManagement");
    }

    [HttpPost]
    public async Task<IActionResult> UpdateCategoryImage(int menuItemId, int? selectedOption)
    {
        var form = Request.Form;
        try
        {
            await _menuManageService.UpdateItemImageAsync(menuItemId,selectedOption, form);
            TempData["Success"] = "Успешна промяна на снимка за категория.";
        }
        catch (Exception e)
        {
            TempData["Error"] = "Неуспешна промяна на снимка за категория." + e.Message;
        }
        return RedirectToAction("GetMenuManagement");
    }
}