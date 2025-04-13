using EMatrix.DataModels.DTOs;
using Microsoft.AspNetCore.Http;

namespace EMatrix.DatabaseServices.Admin.Interfaces;

public interface IUpdateDatabaseService
{
    Task<string> ReadFileAsync(IFormCollection? form);
    
    (List<CsvRecord>,  Dictionary<string, string>,  Dictionary<string, string>) GetRecordsWithCategoryTree(string fileContent);
    
    Task<bool> DatabaseUpdateCategories(Dictionary<string, string> categories);
    
    Task<bool> DatabaseUpdateSubcategories(Dictionary<string, string> subcategories);
    
    Task<bool> DatabaseUpdateRecords(List<CsvRecord> records);
    
}