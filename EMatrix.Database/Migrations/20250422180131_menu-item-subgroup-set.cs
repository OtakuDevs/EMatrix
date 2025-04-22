using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EMatrix.Database.Migrations
{
    /// <inheritdoc />
    public partial class menuitemsubgroupset : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MenuItemSubGroupSet",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    MenuItemId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuItemSubGroupSet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MenuItemSubGroupSet_MenuItems_MenuItemId",
                        column: x => x.MenuItemId,
                        principalTable: "MenuItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MenuItemSubGroupSetEntry",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SubCategoryId = table.Column<int>(type: "integer", nullable: false),
                    SubCategoryName = table.Column<string>(type: "text", nullable: false),
                    MenuItemSubGroupSetId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuItemSubGroupSetEntry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MenuItemSubGroupSetEntry_MenuItemSubGroupSet_MenuItemSubGro~",
                        column: x => x.MenuItemSubGroupSetId,
                        principalTable: "MenuItemSubGroupSet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MenuItemSubGroupSet_MenuItemId",
                table: "MenuItemSubGroupSet",
                column: "MenuItemId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuItemSubGroupSetEntry_MenuItemSubGroupSetId",
                table: "MenuItemSubGroupSetEntry",
                column: "MenuItemSubGroupSetId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MenuItemSubGroupSetEntry");

            migrationBuilder.DropTable(
                name: "MenuItemSubGroupSet");
        }
    }
}
