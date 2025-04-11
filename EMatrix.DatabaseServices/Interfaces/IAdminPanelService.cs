using EMatrix.DataModels.DTOs;
using Microsoft.AspNetCore.Http;

namespace EMatrix.DatabaseServices.Interfaces;

public interface IAdminPanelService
{
    Task<string> ReadFileAsync(IFormCollection? form);
    
    (List<CsvRecord>, List<string>) MatchRecordsToDtoList(string fileContent);
    
    Task<bool> UpdateDatabaseFromDtos(List<CsvRecord> records);
}