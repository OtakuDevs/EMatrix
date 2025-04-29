using EMatrix.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static EMatrix.Constants.ConfigurationConstants;

namespace EMatrix.Database.Configurations;

public class SeedConfiguration : IEntityTypeConfiguration<Menu>
{
    public void Configure(EntityTypeBuilder<Menu> builder)
    {
        builder.HasData(new Menu() { Id = 1, Name = "Products menu" });
    }
}