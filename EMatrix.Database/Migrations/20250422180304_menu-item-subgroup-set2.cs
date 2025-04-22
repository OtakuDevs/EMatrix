using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMatrix.Database.Migrations
{
    /// <inheritdoc />
    public partial class menuitemsubgroupset2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuItemSubGroupSet_MenuItems_MenuItemId",
                table: "MenuItemSubGroupSet");

            migrationBuilder.DropForeignKey(
                name: "FK_MenuItemSubGroupSetEntry_MenuItemSubGroupSet_MenuItemSubGro~",
                table: "MenuItemSubGroupSetEntry");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MenuItemSubGroupSetEntry",
                table: "MenuItemSubGroupSetEntry");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MenuItemSubGroupSet",
                table: "MenuItemSubGroupSet");

            migrationBuilder.RenameTable(
                name: "MenuItemSubGroupSetEntry",
                newName: "MenuItemSubGroupSetEntries");

            migrationBuilder.RenameTable(
                name: "MenuItemSubGroupSet",
                newName: "MenuItemSubGroupSets");

            migrationBuilder.RenameIndex(
                name: "IX_MenuItemSubGroupSetEntry_MenuItemSubGroupSetId",
                table: "MenuItemSubGroupSetEntries",
                newName: "IX_MenuItemSubGroupSetEntries_MenuItemSubGroupSetId");

            migrationBuilder.RenameIndex(
                name: "IX_MenuItemSubGroupSet_MenuItemId",
                table: "MenuItemSubGroupSets",
                newName: "IX_MenuItemSubGroupSets_MenuItemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MenuItemSubGroupSetEntries",
                table: "MenuItemSubGroupSetEntries",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MenuItemSubGroupSets",
                table: "MenuItemSubGroupSets",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MenuItemSubGroupSetEntries_MenuItemSubGroupSets_MenuItemSub~",
                table: "MenuItemSubGroupSetEntries",
                column: "MenuItemSubGroupSetId",
                principalTable: "MenuItemSubGroupSets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MenuItemSubGroupSets_MenuItems_MenuItemId",
                table: "MenuItemSubGroupSets",
                column: "MenuItemId",
                principalTable: "MenuItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuItemSubGroupSetEntries_MenuItemSubGroupSets_MenuItemSub~",
                table: "MenuItemSubGroupSetEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_MenuItemSubGroupSets_MenuItems_MenuItemId",
                table: "MenuItemSubGroupSets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MenuItemSubGroupSets",
                table: "MenuItemSubGroupSets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MenuItemSubGroupSetEntries",
                table: "MenuItemSubGroupSetEntries");

            migrationBuilder.RenameTable(
                name: "MenuItemSubGroupSets",
                newName: "MenuItemSubGroupSet");

            migrationBuilder.RenameTable(
                name: "MenuItemSubGroupSetEntries",
                newName: "MenuItemSubGroupSetEntry");

            migrationBuilder.RenameIndex(
                name: "IX_MenuItemSubGroupSets_MenuItemId",
                table: "MenuItemSubGroupSet",
                newName: "IX_MenuItemSubGroupSet_MenuItemId");

            migrationBuilder.RenameIndex(
                name: "IX_MenuItemSubGroupSetEntries_MenuItemSubGroupSetId",
                table: "MenuItemSubGroupSetEntry",
                newName: "IX_MenuItemSubGroupSetEntry_MenuItemSubGroupSetId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MenuItemSubGroupSet",
                table: "MenuItemSubGroupSet",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MenuItemSubGroupSetEntry",
                table: "MenuItemSubGroupSetEntry",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MenuItemSubGroupSet_MenuItems_MenuItemId",
                table: "MenuItemSubGroupSet",
                column: "MenuItemId",
                principalTable: "MenuItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MenuItemSubGroupSetEntry_MenuItemSubGroupSet_MenuItemSubGro~",
                table: "MenuItemSubGroupSetEntry",
                column: "MenuItemSubGroupSetId",
                principalTable: "MenuItemSubGroupSet",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
