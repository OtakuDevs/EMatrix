using EMatrix.Database;
using EMatrix.DatabaseServices.Admin.Interfaces;
using EMatrix.ViewModels.Admin;
using Microsoft.EntityFrameworkCore;

namespace EMatrix.DatabaseServices.Admin;

public class ManageInventoryService : IManageInventoryService
{
    private readonly EMatrixDbContext _context;

    public ManageInventoryService(EMatrixDbContext context)
    {
        _context = context;
    }

    public async Task<InventoryIndexViewModel> GetInventoryIndexAsync()
    {
        var records = await _context.InventoryItems
            .Select(r => new InventoryItemViewModel()
            {
                Id = r.Id,
                NameAlias = r.NameAlias,
                Price = r.Price,
                Quantity = r.Quantity,
            })
            .ToListAsync();
        var model = new InventoryIndexViewModel()
        {
            InventoryItems = records
        };
        return model;
    }
}