using EMatrix.DatabaseServices.Admin.Interfaces;
using Microsoft.AspNetCore.Http;
// ReSharper disable ConvertToPrimaryConstructor

namespace EMatrix.DatabaseServices.Admin;

public class AdminPanelService : IAdminPanelService
{
    private readonly IUpdateDatabaseService _updateDatabaseService;

    public AdminPanelService(IUpdateDatabaseService updateDatabaseService)
    {
        _updateDatabaseService = updateDatabaseService;
    }


    public async Task<(bool, List<string>)> UpdateDatabaseAsync(IFormCollection form)
    {
        string fileContent = await _updateDatabaseService.ReadFileAsync(form);
        var (records, categories, subcategories, unmatchedRecords) =
            _updateDatabaseService.GetRecordsWithCategoryTree(fileContent);
        var updateCategories = await _updateDatabaseService.DatabaseUpdateCategories(categories);
        if (updateCategories)
        {
            var updateSubcategories = await _updateDatabaseService.DatabaseUpdateSubcategories(subcategories);
            if (updateSubcategories)
            {
                var updateRecords = await _updateDatabaseService.DatabaseUpdateRecords(records);
                if (updateRecords)
                    return (true, unmatchedRecords);
            }
        }
        return (false, new List<string>());
    }
}