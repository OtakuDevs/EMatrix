using EMatrix.ViewModels.Admin;

namespace EMatrix.DatabaseServices.Admin.Interfaces;

public interface IMenuManageService
{
    Task<MenuAdminViewModel> GetMenu(int id);
}