

using EMatrix.UtilityServices.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EMatrix.Controllers;

public class ToolsController : Controller
{
    private readonly IToolsService _toolsService;

    public ToolsController(IToolsService toolsService)
    {
        _toolsService = toolsService;
    }

    // GET
    public IActionResult ResistorColorCodeCalculator()
    {
        var model = _toolsService.GetResistorColorCodeModel();
        return View(model);
    }
}