using Microsoft.EntityFrameworkCore.Migrations;

namespace WebMVC.Data.Migrations.Distributing
{
    public partial class AddBalanceIdToCommission : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BalanceId",
                schema: "distributing",
                table: "commissions",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BalanceId",
                schema: "distributing",
                table: "commissions");
        }
    }
}
