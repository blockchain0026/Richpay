using Microsoft.EntityFrameworkCore.Migrations;

namespace WebMVC.Data.Migrations
{
    public partial class TraderAndShopCommissionRealized : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ShopCommissionRealized",
                table: "orderEntrys",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TraderCommissionRealized",
                table: "orderEntrys",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShopCommissionRealized",
                table: "orderEntrys");

            migrationBuilder.DropColumn(
                name: "TraderCommissionRealized",
                table: "orderEntrys");
        }
    }
}
