using EMatrix.DatabaseServices.Admin.Interfaces;
using Microsoft.AspNetCore.Mvc;
using EMatrix.Constants;

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
    public async Task<IActionResult> AddMenuItem(string name)
    {
        var result = await _menuManageService.AddMenuItemAsync(name);
        if (!result)
            TempData["Error"] = $"Неуспешно добавяне на категория: {name}";
        else
            TempData["Success"] = $"Успешно добавяне на категория: {name}";

        return RedirectToAction("GetMenuManagement");
    }

    [HttpPost]
    public async Task<IActionResult> RenameMenuItem(int id, string name)
    {
        try
        {
            await _menuManageService.RenameMenuItemAsync(id, name);
        }
        catch (Exception)
        {
            TempData["Error"] = "Неуспешно преименуване на категория.";
            return RedirectToAction("GetMenuManagement");
        }

        TempData["Success"] = "Успешно преименуване на категория.";
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
    public async Task<IActionResult> UpdateMenuItemAssignments(int menuItemId, string[] selectedCategories, string[] selectedSubCategories)
    {
        try
        {
            await _menuManageService.UpdateMenuItemAssignmentsAsync(menuItemId, selectedCategories, selectedSubCategories);
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
    public async Task<IActionResult> UpdateCategoryImage(int menuItemId)
    {
        var form = Request.Form;
        try
        {
            await _menuManageService.UpdateMenuItemImageAsync(menuItemId, form);
            TempData["Success"] = "Успешна промяна на снимка за категория.";
        }
        catch (Exception e)
        {
            TempData["Error"] = "Неуспешна промяна на снимка за категория." + e.Message;
        }
        return RedirectToAction("GetMenuManagement");
    }
}