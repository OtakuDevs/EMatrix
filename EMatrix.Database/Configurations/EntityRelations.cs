using EMatrix.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EMatrix.Database.Configurations;

public class EntityRelations :
    IEntityTypeConfiguration<Category>,
    IEntityTypeConfiguration<SubCategory>,
    IEntityTypeConfiguration<MenuItemCategory>,
    IEntityTypeConfiguration<MenuItemSubCategory>,
    IEntityTypeConfiguration<MenuItem>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder
            .HasMany(c => c.InventoryItems)
            .WithOne(i => i.Category)
            .HasForeignKey(i => i.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder
            .HasMany(c => c.SubCategories)
            .WithOne(sc => sc.Category)
            .HasForeignKey(sc => sc.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    public void Configure(EntityTypeBuilder<SubCategory> builder)
    {
        builder
            .HasMany(c => c.InventoryItems)
            .WithOne(i => i.SubCategory)
            .HasForeignKey(i => i.SubCategoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    public void Configure(EntityTypeBuilder<MenuItem> builder)
    {
        builder
            .HasOne(i => i.Menu)
            .WithMany(m => m.MenuItems)
            .HasForeignKey(i => i.MenuId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    public void Configure(EntityTypeBuilder<MenuItemCategory> builder)
    {
        builder.HasKey(mc => new { mc.MenuItemId, mc.CategoryId });
    }

    public void Configure(EntityTypeBuilder<MenuItemSubCategory> builder)
    {
        builder.HasKey(msc => new { msc.MenuItemId, msc.SubCategoryId });
    }
}