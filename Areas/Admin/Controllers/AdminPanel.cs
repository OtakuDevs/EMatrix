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

    [HttpPost]
    public async Task<IActionResult> UpdateDatabase()
    { 
        var form = await Request.ReadFormAsync();
        var result = await _adminPanelService.UpdateDatabaseAsync(form);
        
        if(!result)
            Console.WriteLine("failed to update database");
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> GetMenuManagement()
    {
        var model = await _menuManageService.GetMenu(1);
        return View(model);
    }
}