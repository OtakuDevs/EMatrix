using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using EMatrix.Database;
using EMatrix.DatabaseServices.Interfaces;
using EMatrix.DataModels;
using EMatrix.DataModels.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EMatrix.DatabaseServices;

public class AdminPanelService : IAdminPanelService
{
    private readonly EMatrixDbContext _context;

    public AdminPanelService(EMatrixDbContext context)
    {
        _context = context;
    }
    public async Task<string> ReadFileAsync(IFormCollection? form)
    {
        var file = form.Files["file"];
        string content = string.Empty;
        
        // Register encoding provider for Windows-1251 support
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        await using (var stream = file.OpenReadStream())
        using (var reader = new StreamReader(stream, Encoding.GetEncoding("windows-1251")))
        {
            content = await reader.ReadToEndAsync();
        }
        return content;
    }

    public (List<CsvRecord>, List<string>) MatchRecordsToDtoList(string fileContent)
    {
        string pattern =
            @"^\s*(?<ОсновнаГрупа>.+?)\s*\t{1}(?<Подгрупа>.+?)\s*\t{1}(?<Код>\d{6})\t{1}(?<Име>.+?)\t{1}(?<Описание>.*?)\s*\t?(?<Мярка>БР\.|М\.|Л\.)\s*(?<Цена>\d+(?:\.\d+)?)\s*\t{1}\s*(?<Количество>-?\d+(?:\.\d+)?)\s*$";

        Regex regex = new Regex(pattern, RegexOptions.Multiline);
        var records = new List<CsvRecord>();
        var unmatchedRecords = new List<string>();
        
        var processedLines = ProcessLinesToFullRows(fileContent, regex);
        (records, unmatchedRecords) = MapLinesToLists(processedLines, regex);
        return (records, unmatchedRecords);
    }

    public async Task<bool> UpdateDatabaseFromDtos(List<CsvRecord> records)
    {
        var inventoryItems = await _context.InventoryItems.ToListAsync();
        var categories = await _context.Categories.ToListAsync();
        var subCategories = await _context.SubCategories.ToListAsync();

        var newCategories = new List<Category>();
        var newSubCategories = new List<SubCategory>();
        var newInventoryItems = new List<InventoryItem>();
        foreach (var record in records)
        {
            var item = inventoryItems.FirstOrDefault(i => i.Id == record.Код);
            if (item != null)
            {
                item.Name = record.Име;
                item.Description = record.Описание;
                item.Unit = record.Мярка;
                item.Price = record.Цена;
                item.Quantity = record.Количество;
            }
            else
            {
                item = new InventoryItem
                {
                    Id = record.Код,
                    Name = record.Име,
                    Description = record.Описание,
                    Unit = record.Мярка,
                    Price = record.Цена,
                    Quantity = record.Количество
                };
                newInventoryItems.Add(item);
            }

            var categoryId = record.Код.ToString().Substring(0, 2);
            var category = categories.FirstOrDefault(c => c.Id == int.Parse(categoryId))
                           ?? newCategories.FirstOrDefault(c => c.Id == int.Parse(categoryId));

            if (category == null)
            {
                category = new Category
                {
                    Id = int.Parse(categoryId),
                    Name = record.ОсновнаГрупа,
                    Alias = record.ОсновнаГрупа
                };
                newCategories.Add(category);
            }

            var subCategoryId = record.Код.ToString().Substring(0, 4);
            var subCategory = subCategories.FirstOrDefault(sc => sc.Id == int.Parse(subCategoryId))
                              ?? newSubCategories.FirstOrDefault(sc => sc.Id == int.Parse(subCategoryId));

            if (subCategory == null)
            {
                subCategory = new SubCategory
                {
                    Id = int.Parse(subCategoryId),
                    Name = record.Подгрупа,
                    Alias = record.Подгрупа,
                    Category = category // set reference properly
                };
                newSubCategories.Add(subCategory);
            }

            item.Category = category;
            item.SubCategory = subCategory;
        }
        
        try
        {
            // First: add Categories
            _context.Categories.AddRange(newCategories);
            await _context.SaveChangesAsync(); // now CategoryIds are set

            // Now: add SubCategories
            _context.SubCategories.AddRange(newSubCategories);
            await _context.SaveChangesAsync();

            // Then: add/update InventoryItems
            _context.InventoryItems.UpdateRange(inventoryItems);
            _context.InventoryItems.AddRange(newInventoryItems);
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return false;
        }

        return true;
    }

    private List<string> ProcessLinesToFullRows(string fileContent, Regex regex)
    {
        var lines = fileContent.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        var processedLines = new List<string>();

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
        return processedLines;
    }

    private (List<CsvRecord>, List<string>) MapLinesToLists(List<string> processedLines, Regex regex)
    {
        List<CsvRecord> records = new List<CsvRecord>();
        List<string> unmatchedRecords = new List<string>();
        
        foreach (var processedLine in processedLines)
        {
            var match = regex.Match(processedLine);

            if (match.Success)
            {
                records.Add(new CsvRecord
                {
                    ОсновнаГрупа = match.Groups["ОсновнаГрупа"].Value.Trim(),
                    Подгрупа = match.Groups["Подгрупа"].Value.Trim(),
                    Код = int.Parse(match.Groups["Код"].Value.Trim()),
                    Име = match.Groups["Име"].Value.Trim(),
                    Описание = match.Groups["Описание"].Value.Trim(),
                    Мярка = match.Groups["Мярка"].Value.Trim(),
                    Цена = float.Parse(match.Groups["Цена"].Value.Trim(), CultureInfo.InvariantCulture),
                    Количество = float.Parse(match.Groups["Количество"].Value.Trim(), CultureInfo.InvariantCulture)
                });
            }
            else
            {
                unmatchedRecords.Add(processedLine);
            }
        }
        return (records, unmatchedRecords);
    }
    
}