using EMatrix.Constants;
using EMatrix.DatabaseServices.Admin.Interfaces;
using Microsoft.AspNetCore.Mvc;

// ReSharper disable ConvertToPrimaryConstructor

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
        return View(new List<string>());
    }

    [HttpPost]
    public async Task<IActionResult> UpdateDatabaseAsync()
    {
        try
        {
            var form = await Request.ReadFormAsync();
            if (form.Count == 0)
            {
                TempData["Error"] = StatusMessages.UpdateDatabaseFormEmpty;
                return View(new List<string>());
            }

            var (result, unmatchedRecords) = await _adminPanelService.UpdateDatabaseAsync(form);

            if (!result)
            {
                TempData["Error"] = StatusMessages.UpdateDatabaseFailed;
            }
            else
            {
                TempData["Success"] = StatusMessages.UpdateDatabaseSuccess;
            }

            return View(unmatchedRecords);
        }
        catch (FormatException ex)
        {
            TempData["Error"] = StatusMessages.UpdateDatabaseFileNotSupported;
            return View(new List<string> { ex.Message });
        }
        catch (Exception)
        {
            TempData["Error"] = StatusMessages.UpdateDatabaseException;
            return View(new List<string>());
        }
    }
}