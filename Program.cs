using EMatrix.Database;
using EMatrix.DatabaseServices.Admin;
using EMatrix.DatabaseServices.Admin.Interfaces;
using EMatrix.DataModels;
using EMatrix.UtilityServices;
using EMatrix.UtilityServices.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EMatrix;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddDbContext<EMatrixDbContext>(options =>
            options.UseNpgsql(connectionString: builder.Configuration.GetConnectionString("EMatrixDbContext")));
        
        builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
            {
                options.Lockout.AllowedForNewUsers = false;
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 12;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<EMatrixDbContext>();

        // Add services to the container.
        builder.Services.AddControllersWithViews();
        
        //Register Services
        builder.Services.AddScoped<IAdminPanelService, AdminPanelService>();
        builder.Services.AddScoped<IUpdateDatabaseService, UpdateDatabaseService>();
        builder.Services.AddScoped<IMenuManageService, MenuManageService>();
        builder.Services.AddScoped<IManageInventoryService, ManageInventoryService>();
        builder.Services.AddScoped<IToolsService, ToolsService>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "areaRoute",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        });

        app.Run();
    }
}