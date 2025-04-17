using System.Text.Json.Nodes;
using EMatrix.ViewModels.Admin;

namespace EMatrix.DatabaseServices.Admin.Interfaces;

public interface IManageInventoryService
{
    Task<InventoryIndexViewModel> GetInventoryIndexAsync(int page = 1, string search = "", string category = "", string subCategory = "");

    Task<JsonObject?>GetInventoryItemByIdAsync(string id);

    Task UpdateInventoryItemAsync(string id, string? nameAlias, string? descriptionAlias);
}