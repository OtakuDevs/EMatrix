using EMatrix.ViewModels.Admin;

namespace EMatrix.DatabaseServices.Admin.Interfaces;

public interface IManageInventoryService
{
    Task<InventoryIndexViewModel> GetInventoryIndexAsync();
}