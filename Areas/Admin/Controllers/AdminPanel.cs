using EMatrix.DatabaseServices.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EMatrix.Areas.Admin.Controllers;

[Area("Admin")]
public class AdminPanel : Controller
{
    private readonly IAdminPanelService _adminPanelService;

    public AdminPanel(IAdminPanelService adminPanelService)
    {
        _adminPanelService = adminPanelService;
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
}