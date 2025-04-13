using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMatrix.Database.Migrations
{
    /// <inheritdoc />
    public partial class AliasProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DescriptionAlias",
                table: "InventoryItems",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NameAlias",
                table: "InventoryItems",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DescriptionAlias",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "NameAlias",
                table: "InventoryItems");
        }
    }
}
