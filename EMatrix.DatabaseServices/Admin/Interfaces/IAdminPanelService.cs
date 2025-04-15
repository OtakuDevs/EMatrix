using Microsoft.AspNetCore.Http;

namespace EMatrix.DatabaseServices.Admin.Interfaces;

public interface IAdminPanelService
{
    Task<(bool result, List<string>)> UpdateDatabaseAsync(IFormCollection form);
    
}