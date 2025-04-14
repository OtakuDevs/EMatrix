using EMatrix.Constants;
using EMatrix.DatabaseServices.Admin.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EMatrix.Areas.Admin.Controllers;

[Area("Admin")]
public class AdminPanel : Controller
{
    private readonly IAdminPanelService _adminPanelService;
    private readonly IMenuManageService _menuManageService;

    public AdminPanel(IAdminPanelService adminPanelService, IMenuManageService menuManageService)
    {
        _adminPanelService = adminPanelService;
        _menuManageService = menuManageService;
    }
    // GET
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public IActionResult UpdateDatabase()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> UpdateDatabaseAsync()
    { 
        var form = await Request.ReadFormAsync();
        var result = await _adminPanelService.UpdateDatabaseAsync(form);
        
        if(!result)
            Console.WriteLine("failed to update database");
        return View();
    }

    public async Task<IActionResult> GetMenuManagement()
    {
        var model = await _menuManageService.GetMenu(ConfigurationConstants.MenuId);
        return View(model);
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
        catch (Exception e)
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
        catch (Exception e)
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
        var model = await _menuManageService.GetMenuItemModelAsync(menuItemId);
        return View(model);
    }
}