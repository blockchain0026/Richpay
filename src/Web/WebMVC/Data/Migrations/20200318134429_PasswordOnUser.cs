using Microsoft.EntityFrameworkCore.Migrations;

namespace WebMVC.Data.Migrations
{
    public partial class PasswordOnUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Password",
                schema: "application",
                table: "traders",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Password",
                schema: "application",
                table: "traderAgents",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Password",
                schema: "application",
                table: "shops",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Password",
                schema: "application",
                table: "shopAgents",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Password",
                schema: "application",
                table: "traders");

            migrationBuilder.DropColumn(
                name: "Password",
                schema: "application",
                table: "traderAgents");

            migrationBuilder.DropColumn(
                name: "Password",
                schema: "application",
                table: "shops");

            migrationBuilder.DropColumn(
                name: "Password",
                schema: "application",
                table: "shopAgents");
        }
    }
}
