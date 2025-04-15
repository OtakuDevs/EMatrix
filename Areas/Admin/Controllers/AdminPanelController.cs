using EMatrix.DatabaseServices.Admin.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EMatrix.Areas.Admin.Controllers;

[Area("Admin")]
public class AdminPanelController : Controller
{
    private readonly IAdminPanelService _adminPanelService;

    public AdminPanelController(IAdminPanelService adminPanelService)
    {
        _adminPanelService = adminPanelService;
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
}