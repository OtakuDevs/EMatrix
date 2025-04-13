using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using EMatrix.Database;
using EMatrix.DatabaseServices.Admin.Interfaces;
using EMatrix.DataModels;
using EMatrix.DataModels.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EMatrix.DatabaseServices.Admin;

public class UpdateDatabaseService : IUpdateDatabaseService
{
    private readonly EMatrixDbContext _context;

    public UpdateDatabaseService(EMatrixDbContext context)
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
    
    public (List<CsvRecord>,  Dictionary<string, string>, Dictionary<string, string>) GetRecordsWithCategoryTree(string fileContent)
    {
        string pattern =
            @"^\s*(?<ОсновнаГрупа>.+?)\s*\t{1}(?<Подгрупа>.+?)\s*\t{1}(?<Код>\d{6})\t{1}(?<Име>.+?)\t{1}(?<Описание>.*?)\s*\t?(?<Мярка>БР\.|М\.|Л\.)\s*(?<Цена>\d+(?:\.\d+)?)\s*\t{1}\s*(?<Количество>-?\d+(?:\.\d+)?)\s*$";

        Regex regex = new Regex(pattern, RegexOptions.Multiline);
        var records = new List<CsvRecord>();
        var unmatchedRecords = new List<string>();
        
        var processedLines = ProcessLinesToFullRows(fileContent, regex);
        records = MapToCsvRecords(processedLines, regex);
        Dictionary<string, string> Categories = records
            .GroupBy(r => r.Код.ToString().Substring(0, 2))
            .ToDictionary(r => r.Key, g => g.First().ОсновнаГрупа);

        Dictionary<string, string> SubCategories = records
            .GroupBy(r => r.Код.ToString().Substring(0, 4))
            .ToDictionary(r => r.Key, g => g.First().Подгрупа);
        return (records, Categories, SubCategories);
    }

    public async Task<bool> DatabaseUpdateCategories(Dictionary<string, string> categories)
    {
        var existingCategories = await _context.Categories.ToListAsync();
        List<Category> categoriesToRemove = existingCategories
            .Where(r => !categories.ContainsKey(r.Id.ToString()))
            .ToList();
        List<Category> newCategories = new List<Category>();
        foreach (var item in categories)
        {
            var category = existingCategories.FirstOrDefault(c => c.Id == item.Key)
                           ?? newCategories.FirstOrDefault(c => c.Id == item.Key);
            if (category == null)
            {
                category = new Category
                {
                    Id = item.Key,
                    Name = item.Value,
                    Alias = item.Value,
                };
                newCategories.Add(category);
            }
            else
            {
                if (category.Name != item.Value)
                {
                    category.Name = item.Value;
                    category.Alias = item.Value;
                    _context.Categories.Update(category);
                }
            }
        }
        _context.Categories.RemoveRange(categoriesToRemove);
        _context.Categories.AddRange(newCategories);
        await _context.SaveChangesAsync(); // now CategoryIds are set
        return true;
    }

    public async Task<bool> DatabaseUpdateSubcategories(Dictionary<string, string> subcategories)
    {
        var existingSubCategories = await _context.SubCategories.ToListAsync();
        List<SubCategory> subCategoriesToRemove = existingSubCategories
            .Where(r => !subcategories.ContainsKey(r.Id.ToString()))
            .ToList();
        List<SubCategory> newSubCategories = new List<SubCategory>();
        foreach (var item in subcategories)
        {
            var subCategory = existingSubCategories.FirstOrDefault(c => c.Id == item.Key)
                              ?? newSubCategories.FirstOrDefault(c => c.Id == item.Key);
            if (subCategory == null)
            {
                subCategory = new SubCategory
                {
                    Id = item.Key,
                    Name = item.Value,
                    Alias = item.Value,
                    CategoryId = item.Key.Substring(0, 2),
                };
                newSubCategories.Add(subCategory);
            }
            else
            {
                if (subCategory.Name != item.Value)
                {
                    subCategory.Name = item.Value;
                    subCategory.Alias = item.Value;
                    _context.SubCategories.Update(subCategory);
                }
            }
        }
        _context.SubCategories.RemoveRange(subCategoriesToRemove);
        _context.SubCategories.AddRange(newSubCategories);
        await _context.SaveChangesAsync(); // now CategoryIds are set
        return true;
    }

    public async Task<bool> DatabaseUpdateRecords(List<CsvRecord> records)
    {
        var existingRecords = await _context.InventoryItems.ToListAsync();
        List<InventoryItem> newRecords = new List<InventoryItem>();
        var csvRecordsIds = records.Select(r => r.Код).ToList();
        List<InventoryItem> recordsToRemove = existingRecords.Where(r => !csvRecordsIds.Contains(r.Id)).ToList();

        foreach (var record in records)
        {
            var item = existingRecords.FirstOrDefault(c => c.Id == record.Код)
                       ?? newRecords.FirstOrDefault(c => c.Id == record.Код);
            if (item != null)
            {
                item.Name = record.Име;
                item.Name = item.Name != record.Име ? record.Име : item.NameAlias;
                item.Description = record.Описание;
                item.DescriptionAlias = item.Description != record.Описание ? record.Описание : item.DescriptionAlias;
                item.Unit = record.Мярка;
                item.Price = record.Цена;
                item.Quantity = record.Количество;
                _context.InventoryItems.Update(item);
            }
            else
            {
                item = new InventoryItem
                {
                    Id = record.Код,
                    Name = record.Име,
                    NameAlias = record.Име,
                    Description = record.Описание,
                    DescriptionAlias = record.Описание,
                    Unit = record.Мярка,
                    Price = record.Цена,
                    Quantity = record.Количество,
                    SubCategoryId = record.Код.Substring(0,4),
                    CategoryId = record.Код.Substring(0,2),
                };
                newRecords.Add(item);
            }
        }

        // Explicitly handle the remove and add operations separately
        using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            try
            {
                // Remove items that are not in the CSV records
                _context.InventoryItems.RemoveRange(recordsToRemove);
                await _context.SaveChangesAsync();

                // Add new records
                _context.InventoryItems.AddRange(newRecords);
                await _context.SaveChangesAsync();

                // Commit the transaction
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                // In case of an error, roll back the transaction
                await transaction.RollbackAsync();
                throw; // Re-throw the exception
            }
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

    private List<CsvRecord> MapToCsvRecords(List<string> processedLines, Regex regex)
    {
        List<CsvRecord> records = new List<CsvRecord>();
        
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
        }
        return records;
    }
}