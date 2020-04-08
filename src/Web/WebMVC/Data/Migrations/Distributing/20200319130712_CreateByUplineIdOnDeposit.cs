using Microsoft.EntityFrameworkCore.Migrations;

namespace WebMVC.Data.Migrations.Distributing
{
    public partial class CreateByUplineIdOnDeposit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                schema: "distributing",
                table: "deposits",
                unicode: false,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "CreateByUplineId",
                schema: "distributing",
                table: "deposits",
                unicode: false,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreateByUplineId",
                schema: "distributing",
                table: "deposits");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                schema: "distributing",
                table: "deposits",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldUnicode: false);
        }
    }
}
