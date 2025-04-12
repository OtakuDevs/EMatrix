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
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpPost]
    public async Task<IActionResult> ReadFile()
    {
        var form = await Request.ReadFormAsync();
        var file = form.Files["file"];

        // Register encoding provider for Windows-1251 support
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        using (var stream = file.OpenReadStream())
        using (var reader = new StreamReader(stream, Encoding.GetEncoding("windows-1251")))
        {
            string fileContent = await reader.ReadToEndAsync();

            string pattern =
                @"^\s*(?<ОсновнаГрупа>.+?)\s*\t{1}(?<Подгрупа>.+?)\s*\t{1}(?<Код>\d{6})\t{1}(?<Име>.+?)\t{1}(?<Описание>.*?)\s*\t?(?<Мярка>БР\.|М\.|Л\.)\s*(?<Цена>\d+(?:\.\d+)?)\s*\t{1}\s*(?<Количество>-?\d+(?:\.\d+)?)\s*$";

            Regex regex = new Regex(pattern, RegexOptions.Multiline);
            var records = new List<CsvRecord>();
            var unmatchedRows = new List<string>();

            var lines = fileContent.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            var processedLines = new List<string>();

            // **Step 1: Preprocess lines to reconstruct full rows**
            StringBuilder currentLine = new StringBuilder();
            foreach (var line in lines)
            {
                if (regex.IsMatch(line.Trim()))
                {
                    // If currentLine has accumulated text, save it first
                    if (currentLine.Length > 0)
                    {
                        processedLines.Add(currentLine.ToString().Trim());
                        currentLine.Clear();
                    }

                    processedLines.Add(line.Trim()); // Add the valid row directly
                }
                else
                {
                    // Accumulate partial rows
                    currentLine.Append(" " + line.Trim());
                }
            }

            // Add the last accumulated line (if any)
            if (currentLine.Length > 0)
            {
                processedLines.Add(currentLine.ToString().Trim());
            }

            // **Step 2: Process the cleaned-up rows**
            foreach (var processedLine in processedLines)
            {
                var match = regex.Match(processedLine);

                if (match.Success)
                {
                    var rawId = match.Groups["Код"].Value.Trim();
                    var paddedId = rawId.Length == 6 ? rawId : "0" + rawId;
                    records.Add(new CsvRecord
                    {
                        ОсновнаГрупа = match.Groups["ОсновнаГрупа"].Value.Trim(),
                        Подгрупа = match.Groups["Подгрупа"].Value.Trim(),
                        Код = paddedId,
                        Име = match.Groups["Име"].Value.Trim(),
                        Описание = match.Groups["Описание"].Value.Trim(),
                        Мярка = match.Groups["Мярка"].Value.Trim(),
                        Цена = float.Parse(match.Groups["Цена"].Value.Trim(), CultureInfo.InvariantCulture),
                        Количество = float.Parse(match.Groups["Количество"].Value.Trim(), CultureInfo.InvariantCulture)
                    });
                }
                else
                {
                    unmatchedRows.Add(processedLine);
                }
            }

            var model = new HomeModel()
            {
                Records = records,
                UnmatchedRows = unmatchedRows,
            };
            model.Groups = records
                .GroupBy(r => r.ОсновнаГрупа)
                .ToDictionary(
                    r => r.Key + " - " + r.First().Код.Substring(0, 2),
                    r => r.OrderBy(r => r.Подгрупа).ToList());

            model.GroupDataCodes = 
                model.Groups.ToDictionary(
                    r => r.Key,
                    r => r.Value
                        .GroupBy(p => p.Подгрупа)
                        .ToDictionary(
                            c => c.Key,
                            c => c.First().Код.Substring(2,2 ))); 
            ViewData["UnmatchedRows"] = unmatchedRows;
            return View(model);
        }
    }
}

public class HomeModel
{
    public List<string> UnmatchedRows { get; set; }
    public List<CsvRecord> Records { get; set; }
    public Dictionary<string, List<CsvRecord>> Groups { get; set; }
    
    public Dictionary<string, Dictionary<string, string>> GroupDataCodes { get; set; }
}

public class CsvRecord
{
    public string ОсновнаГрупа { get; set; }
    public string Подгрупа { get; set; }
    public string Код { get; set; }
    public string Име { get; set; }
    public string Описание { get; set; }
    public string Мярка { get; set; }
    public float Цена { get; set; }
    public float Количество { get; set; }
}
