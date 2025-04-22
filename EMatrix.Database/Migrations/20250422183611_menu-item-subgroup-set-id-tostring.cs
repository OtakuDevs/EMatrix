using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMatrix.Database.Migrations
{
    /// <inheritdoc />
    public partial class menuitemsubgroupsetidtostring : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SubCategoryId",
                table: "MenuItemSubGroupSetEntries",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "SubCategoryId",
                table: "MenuItemSubGroupSetEntries",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
