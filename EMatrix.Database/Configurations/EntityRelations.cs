using EMatrix.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EMatrix.Database.Configurations;

public class EntityRelations :
    IEntityTypeConfiguration<Category>,
    IEntityTypeConfiguration<SubCategory>
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
}