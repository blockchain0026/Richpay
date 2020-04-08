using Microsoft.EntityFrameworkCore.Migrations;

namespace WebMVC.Data.Migrations
{
    public partial class HasMultipleIndexOnOrderEntry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_orderEntrys_DateCreated",
                table: "orderEntrys");

            migrationBuilder.AlterColumn<string>(
                name: "TraderId",
                table: "orderEntrys",
                unicode: false,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(max)",
                oldUnicode: false,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ShopId",
                table: "orderEntrys",
                unicode: false,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(max)",
                oldUnicode: false);

            migrationBuilder.CreateIndex(
                name: "IX_orderEntrys_DateCreated_TraderId_ShopId",
                table: "orderEntrys",
                columns: new[] { "DateCreated", "TraderId", "ShopId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_orderEntrys_DateCreated_TraderId_ShopId",
                table: "orderEntrys");

            migrationBuilder.AlterColumn<string>(
                name: "TraderId",
                table: "orderEntrys",
                type: "varchar(max)",
                unicode: false,
                nullable: true,
                oldClrType: typeof(string),
                oldUnicode: false,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ShopId",
                table: "orderEntrys",
                type: "varchar(max)",
                unicode: false,
                nullable: false,
                oldClrType: typeof(string),
                oldUnicode: false);

            migrationBuilder.CreateIndex(
                name: "IX_orderEntrys_DateCreated",
                table: "orderEntrys",
                column: "DateCreated");
        }
    }
}
