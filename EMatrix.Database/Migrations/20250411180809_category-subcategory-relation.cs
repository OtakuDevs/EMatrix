using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMatrix.Database.Migrations
{
    /// <inheritdoc />
    public partial class categorysubcategoryrelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryItems_Subcategories_SubCategoryId",
                table: "InventoryItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Subcategories",
                table: "Subcategories");

            migrationBuilder.RenameTable(
                name: "Subcategories",
                newName: "SubCategories");

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "SubCategories",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<float>(
                name: "Quantity",
                table: "InventoryItems",
                type: "real",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubCategories",
                table: "SubCategories",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_SubCategories_CategoryId",
                table: "SubCategories",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryItems_SubCategories_SubCategoryId",
                table: "InventoryItems",
                column: "SubCategoryId",
                principalTable: "SubCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SubCategories_Categories_CategoryId",
                table: "SubCategories",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryItems_SubCategories_SubCategoryId",
                table: "InventoryItems");

            migrationBuilder.DropForeignKey(
                name: "FK_SubCategories_Categories_CategoryId",
                table: "SubCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SubCategories",
                table: "SubCategories");

            migrationBuilder.DropIndex(
                name: "IX_SubCategories_CategoryId",
                table: "SubCategories");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "SubCategories");

            migrationBuilder.RenameTable(
                name: "SubCategories",
                newName: "Subcategories");

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "InventoryItems",
                type: "integer",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Subcategories",
                table: "Subcategories",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryItems_Subcategories_SubCategoryId",
                table: "InventoryItems",
                column: "SubCategoryId",
                principalTable: "Subcategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
