using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Mvc;
using EMatrix.Models;
using Microsoft.AspNetCore.SignalR;

namespace EMatrix.Controllers;

public class HomeController : Controller
{
    public HomeController()
    {

    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy(string[]? args)
    {
        return View();
    }


    public IActionResult LoadPartial(string partialName)
    {
        if (string.IsNullOrWhiteSpace(partialName)) return BadRequest();

        return PartialView(partialName);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error(int statusCode, string? message)
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
