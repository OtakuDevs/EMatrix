using EMatrix.Database.Configurations;
using EMatrix.DataModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EMatrix.Database;

public class EMatrixDbContext : IdentityDbContext<ApplicationUser>
{
    public EMatrixDbContext(DbContextOptions<EMatrixDbContext> options) : base(options){}
    
    public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    
    public DbSet<Category> Categories { get; set; }
    
    public DbSet<SubCategory> SubCategories { get; set; }
    
    public DbSet<InventoryItem> InventoryItems { get; set; }

    public DbSet<Menu> Menus { get; set; }

    public DbSet<MenuItem> MenuItems { get; set; }

    public DbSet<MenuItemCategory> MenuItemCategories { get; set; }

    public DbSet<MenuItemSubCategory> MenuItemSubCategories { get; set; }

    public DbSet<MenuItemSubGroupSet> MenuItemSubGroupSets { get; set; }

    public DbSet<MenuItemSubGroupSetEntry> MenuItemSubGroupSetEntries { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        var entityRelations = new EntityRelations();
        builder.ApplyConfiguration<Category>(entityRelations);
        builder.ApplyConfiguration<SubCategory>(entityRelations);
        builder.ApplyConfiguration<MenuItemCategory>(entityRelations);
        builder.ApplyConfiguration<MenuItemSubCategory>(entityRelations);
        builder.ApplyConfiguration<MenuItem>(entityRelations);

        var seedConfiguration = new SeedConfiguration();
        builder.ApplyConfiguration<Menu>(seedConfiguration);
        base.OnModelCreating(builder);
    }
}