using EMatrix.Database;
using EMatrix.DatabaseServices.Admin.Interfaces;
using EMatrix.DataModels;
using EMatrix.ViewModels.Admin;
using Microsoft.EntityFrameworkCore;

namespace EMatrix.DatabaseServices.Admin;

public class MenuManageService : IMenuManageService
{
    private readonly EMatrixDbContext _context;

    public MenuManageService(EMatrixDbContext context)
    {
        _context = context;
    }

    public async Task<MenuAdminViewModel> GetMenu(int id)
    {
        var menu = await _context.Menus
            .Include(r => r.MenuItems).FirstOrDefaultAsync(m => m.Id == id);
        if(menu == null)
            throw new KeyNotFoundException();
        var menuItems = new Dictionary<int, string>();
        foreach (var item in menu.MenuItems)
        {
            menuItems.Add(item.Id, item.Name);
        }

        var model = new MenuAdminViewModel()
        {
            MenuItems = menuItems,
        };
        return model;
    }
}