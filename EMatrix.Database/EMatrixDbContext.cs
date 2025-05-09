﻿using System.Text.RegularExpressions;
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

    public DbSet<MenuOption> MenuOptions { get; set; }

    public DbSet<MenuOptionChild> MenuOptionChildren { get; set; }

    public DbSet<SubGroupSet> SubGroupSets { get; set; }

    public DbSet<SubGroupSetItem> SubGroupSetItems { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        var entityRelations = new EntityRelations();
        builder.ApplyConfiguration<Category>(entityRelations);
        builder.ApplyConfiguration<SubCategory>(entityRelations);
        builder.ApplyConfiguration<MenuItem>(entityRelations);

        var seedConfiguration = new SeedConfiguration();
        builder.ApplyConfiguration<Menu>(seedConfiguration);
        base.OnModelCreating(builder);
    }
}