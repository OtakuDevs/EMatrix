using System.Text.Json.Nodes;
using EMatrix.Database;
using EMatrix.DatabaseServices.Admin.Interfaces;
using EMatrix.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EMatrix.DatabaseServices.Admin;

public class ManageInventoryService : IManageInventoryService
{
    private readonly EMatrixDbContext _context;

    public ManageInventoryService(EMatrixDbContext context)
    {
        _context = context;
    }

    public async Task<InventoryIndexViewModel> GetInventoryIndexAsync(int page = 1, string search = "", string category = "", string subCategory = "")
    {
        const int pageSize = 10;
        var skip = (page - 1) * pageSize;
        var availableCategories = await _context.Categories.ToListAsync();
        var availableSubCategories = await _context.SubCategories.ToListAsync();

        //Base query
        var query = _context.InventoryItems
            .Include(i => i.Category)
            .Include(i => i.SubCategory)
            .AsQueryable();

        // Apply select filter
        if(!string.IsNullOrEmpty(category))
            query = query.Where(i => i.Id.StartsWith(category));

        if(!string.IsNullOrEmpty(subCategory))
            query = query.Where(i => i.Id.StartsWith(subCategory));

        //Apply search filter if provided
        if (!string.IsNullOrEmpty(search))
        {
            string[] tokens = search
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.ToLower())
                .ToArray();

            query = query.Where(item =>

                // every token must be found in *one* of the text columns
                tokens.All(t =>
                    item.Name.ToLower().Contains(t) ||
                    item.NameAlias.ToLower().Contains(t) ||
                    item.Description.ToLower().Contains(t) ||
                    item.DescriptionAlias.ToLower().Contains(t) ||
                    item.SubCategory.Name.ToLower().Contains(t) ||
                    item.SubCategory.Alias.ToLower().Contains(t) ||
                    item.Category.Name.ToLower().Contains(t) ||
                    item.Category.Alias.ToLower().Contains(t)
                )
                || EF.Functions.Like(item.Id, $"%{search}%")
            ).OrderBy(i => i.NameAlias);
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
            AvailableCategories = availableCategories.Select(c => new SelectListItem() { Text = c.Name, Value = c.Id }).ToList(),
            AvailableSubCategories = availableSubCategories.Select(c => new SelectListItem() { Text = c.Name, Value = c.Id }).ToList(),
            CurrentPage = page,
            TotalPages = (int)Math.Ceiling((double)totalPages / pageSize),
            SearchTerm = search,
            CategoryFilter = category,
            SubCategoryFilter = subCategory
        };
        return model;
    }

    public async Task<InventoryGroupsViewModel> GetGroupsIndexAsync()
    {
        var groups = await _context.Categories
            .Include(c => c.SubCategories)
            .ToListAsync();
        var model = new InventoryGroupsViewModel()
        {
            Groups = groups.Select(g => new GroupViewModel()
            {
                Id = g.Id,
                Name = g.Name,
                Alias = g.Alias,
                SubGroups = g.SubCategories.OrderBy(s => s.Id).Select(s => new SubGroupViewModel()
                {
                    Id = s.Id,
                    Name = s.Name,
                    Alias = s.Alias,
                }).ToList(),
            }).ToList()
        };
        return model;
    }

    public async Task UpdateGroupAsync(string id, string type, string nameAlias)
    {
        switch (type)
        {
            case "group":
            {
                var category = await _context.Categories.FindAsync(id);
                if (category == null)
                    throw new KeyNotFoundException();
                category.Alias = nameAlias;
                break;
            }
            case "subgroup":
            {
                var subCategory = await _context.SubCategories.FindAsync(id);
                if (subCategory == null)
                    throw new KeyNotFoundException();
                subCategory.Alias = nameAlias;
                break;
            }
        }

        await _context.SaveChangesAsync();
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

    public async Task UpdateInventoryItemAsync(string id, string? nameAlias, string? descriptionAlias, float quantity, float price)
    {
        var item = await _context.InventoryItems.FirstOrDefaultAsync(i => i.Id == id);
        if (item == null)
            throw new KeyNotFoundException();
        item.NameAlias = string.IsNullOrEmpty(nameAlias) ? item.NameAlias : nameAlias;
        item.Description = string.IsNullOrEmpty(descriptionAlias) ? item.Description : descriptionAlias;
        item.Quantity = quantity;
        item.Price = price;
        _context.InventoryItems.Update(item);
        await _context.SaveChangesAsync();
    }
}