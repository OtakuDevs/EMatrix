using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EMatrix.Database.Migrations
{
    /// <inheritdoc />
    public partial class MenuCompleteOverhaul : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MenuItemCategories");

            migrationBuilder.DropTable(
                name: "MenuItemSubCategories");

            migrationBuilder.DropTable(
                name: "MenuItemSubGroupSetEntries");

            migrationBuilder.DropTable(
                name: "MenuItemSubGroupSets");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Menus",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "MenuOptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Icon = table.Column<string>(type: "text", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    MenuItemId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MenuOptions_MenuItems_MenuItemId",
                        column: x => x.MenuItemId,
                        principalTable: "MenuItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubGroupSets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubGroupSets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MenuOptionChildren",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DisplayName = table.Column<string>(type: "text", nullable: true),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    MenuOptionId = table.Column<int>(type: "integer", nullable: false),
                    SubGroupId = table.Column<string>(type: "text", nullable: true),
                    SubGroupSetId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuOptionChildren", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MenuOptionChildren_MenuOptions_MenuOptionId",
                        column: x => x.MenuOptionId,
                        principalTable: "MenuOptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MenuOptionChildren_SubCategories_SubGroupId",
                        column: x => x.SubGroupId,
                        principalTable: "SubCategories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MenuOptionChildren_SubGroupSets_SubGroupSetId",
                        column: x => x.SubGroupSetId,
                        principalTable: "SubGroupSets",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SubGroupSetItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SubGroupSetId = table.Column<int>(type: "integer", nullable: false),
                    SubGroupId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubGroupSetItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubGroupSetItems_SubCategories_SubGroupId",
                        column: x => x.SubGroupId,
                        principalTable: "SubCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubGroupSetItems_SubGroupSets_SubGroupSetId",
                        column: x => x.SubGroupSetId,
                        principalTable: "SubGroupSets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Products menu");

            migrationBuilder.CreateIndex(
                name: "IX_MenuOptionChildren_MenuOptionId",
                table: "MenuOptionChildren",
                column: "MenuOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuOptionChildren_SubGroupId",
                table: "MenuOptionChildren",
                column: "SubGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuOptionChildren_SubGroupSetId",
                table: "MenuOptionChildren",
                column: "SubGroupSetId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuOptions_MenuItemId",
                table: "MenuOptions",
                column: "MenuItemId");

            migrationBuilder.CreateIndex(
                name: "IX_SubGroupSetItems_SubGroupId",
                table: "SubGroupSetItems",
                column: "SubGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_SubGroupSetItems_SubGroupSetId",
                table: "SubGroupSetItems",
                column: "SubGroupSetId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MenuOptionChildren");

            migrationBuilder.DropTable(
                name: "SubGroupSetItems");

            migrationBuilder.DropTable(
                name: "MenuOptions");

            migrationBuilder.DropTable(
                name: "SubGroupSets");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Menus");

            migrationBuilder.CreateTable(
                name: "MenuItemCategories",
                columns: table => new
                {
                    MenuItemId = table.Column<int>(type: "integer", nullable: false),
                    CategoryId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuItemCategories", x => new { x.MenuItemId, x.CategoryId });
                    table.ForeignKey(
                        name: "FK_MenuItemCategories_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MenuItemCategories_MenuItems_MenuItemId",
                        column: x => x.MenuItemId,
                        principalTable: "MenuItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MenuItemSubCategories",
                columns: table => new
                {
                    MenuItemId = table.Column<int>(type: "integer", nullable: false),
                    SubCategoryId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuItemSubCategories", x => new { x.MenuItemId, x.SubCategoryId });
                    table.ForeignKey(
                        name: "FK_MenuItemSubCategories_MenuItems_MenuItemId",
                        column: x => x.MenuItemId,
                        principalTable: "MenuItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MenuItemSubCategories_SubCategories_SubCategoryId",
                        column: x => x.SubCategoryId,
                        principalTable: "SubCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MenuItemSubGroupSets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MenuItemId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuItemSubGroupSets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MenuItemSubGroupSets_MenuItems_MenuItemId",
                        column: x => x.MenuItemId,
                        principalTable: "MenuItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MenuItemSubGroupSetEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MenuItemSubGroupSetId = table.Column<int>(type: "integer", nullable: false),
                    SubCategoryId = table.Column<string>(type: "text", nullable: false),
                    SubCategoryName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuItemSubGroupSetEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MenuItemSubGroupSetEntries_MenuItemSubGroupSets_MenuItemSub~",
                        column: x => x.MenuItemSubGroupSetId,
                        principalTable: "MenuItemSubGroupSets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MenuItemCategories_CategoryId",
                table: "MenuItemCategories",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuItemSubCategories_SubCategoryId",
                table: "MenuItemSubCategories",
                column: "SubCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuItemSubGroupSetEntries_MenuItemSubGroupSetId",
                table: "MenuItemSubGroupSetEntries",
                column: "MenuItemSubGroupSetId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuItemSubGroupSets_MenuItemId",
                table: "MenuItemSubGroupSets",
                column: "MenuItemId");
        }
    }
}
