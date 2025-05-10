using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMatrix.Database.Migrations
{
    /// <inheritdoc />
    public partial class menuoptionchildaddiconproperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "MenuOptionChildren",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Icon",
                table: "MenuOptionChildren");
        }
    }
}
