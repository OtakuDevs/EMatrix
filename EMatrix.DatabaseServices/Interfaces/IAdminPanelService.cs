using EMatrix.DataModels.DTOs;
using Microsoft.AspNetCore.Http;

namespace EMatrix.DatabaseServices.Interfaces;

public interface IAdminPanelService
{
    Task<bool> UpdateDatabaseAsync(IFormCollection? form);
    
}