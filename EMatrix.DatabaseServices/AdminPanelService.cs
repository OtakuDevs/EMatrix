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
    private readonly IUpdateDatabaseService _updateDatabaseService;

    public AdminPanelService(IUpdateDatabaseService updateDatabaseService)
    {
        _updateDatabaseService = updateDatabaseService;
    }


    public async Task<bool> UpdateDatabaseAsync(IFormCollection? form)
    {
        try
        {
            string fileContent = await _updateDatabaseService.ReadFileAsync(form);
            var (records, categories, subcategories) = _updateDatabaseService.GetRecordsWithCategoryTree(fileContent);
            var updateCategories = await _updateDatabaseService.DatabaseUpdateCategories(categories);
            if (updateCategories)
            {
                var updateSubcategories = await _updateDatabaseService.DatabaseUpdateSubcategories(subcategories);
                if (updateSubcategories)
                {
                    var updateRecords = await _updateDatabaseService.DatabaseUpdateRecords(records);
                }
            }
        }
        catch (Exception e)
        {
            return false;
        }
        return true;
    }

   

   

    
    
}