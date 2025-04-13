using Microsoft.AspNetCore.Http;

namespace EMatrix.DatabaseServices.Admin.Interfaces;

public interface IAdminPanelService
{
    Task<bool> UpdateDatabaseAsync(IFormCollection? form);
    
}