using System.Text.Json.Nodes;
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

    public async Task<InventoryIndexViewModel> GetInventoryIndexAsync(int page = 1, string search = "")
    {
        const int pageSize = 10;
        var skip = (page - 1) * pageSize;

        //Base query
        var query = _context.InventoryItems.AsQueryable();

        //Apply search filter if provided
        if (!string.IsNullOrEmpty(search))
        {
            var searchTerm = search.ToLowerInvariant();
            query = query.Where(item =>
                EF.Functions.Like(item.Name.ToLower(), $"%{searchTerm}%")
                || EF.Functions.Like(item.NameAlias.ToLower(), $"%{searchTerm}%")
                || EF.Functions.Like(item.Description.ToLower(), $"%{searchTerm}%")
                || EF.Functions.Like(item.DescriptionAlias.ToLower(), $"%{searchTerm}%")
                || EF.Functions.Like(item.Id, $"{searchTerm}%"));
        }

        var totalPages = query.Count();
        var records = await query
            .Skip(skip)
            .Take(pageSize)
            .Select(r => new InventoryItemViewModel()
            {
                Id = r.Id,
                NameAlias = r.NameAlias,
                Price = r.Price,
                Quantity = r.Quantity,
                Unit = r.Unit,
                DescriptionAlias = r.DescriptionAlias,
            })
            .ToListAsync();
        var model = new InventoryIndexViewModel()
        {
            InventoryItems = records,
            CurrentPage = page,
            TotalPages = (int)Math.Ceiling((double)totalPages / pageSize),
            SearchTerm = search
        };
        return model;
    }

    public async Task<JsonObject?> GetInventoryItemByIdAsync(string id)
    {
        var item = await _context.InventoryItems
            .Include(c => c.Category)
            .Include(c => c.SubCategory)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (item == null)
            return null;
        var data = new JsonObject
        {
            ["id"] = item.Id,
            ["name"] = item.Name,
            ["nameAlias"] = item.NameAlias,
            ["description"] = item.Description,
            ["descriptionAlias"] = item.DescriptionAlias,
            ["unit"] = item.Unit,
            ["price"] = item.Price,
            ["quantity"] = item.Quantity,
            ["category"] = item.Category?.Alias,
            ["subCategory"] = item.SubCategory?.Alias
        };
        return data;
    }

    public async Task UpdateInventoryItemAsync(string id, string? nameAlias, string? descriptionAlias)
    {
        var item = await _context.InventoryItems.FirstOrDefaultAsync(i => i.Id == id);
        if (item == null)
            throw new KeyNotFoundException();
        item.NameAlias = string.IsNullOrEmpty(nameAlias) ? item.NameAlias : nameAlias;
        item.Description = string.IsNullOrEmpty(descriptionAlias) ? item.Description : descriptionAlias;
        _context.InventoryItems.Update(item);
        await _context.SaveChangesAsync();
    }
}