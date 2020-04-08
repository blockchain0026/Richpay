using Microsoft.EntityFrameworkCore.Migrations;

namespace WebMVC.Data.Migrations
{
    public partial class HasIndexForUserIdAndDateCreatedOnRunningAccountRecord : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_runningAccountRecords_UserId",
                table: "runningAccountRecords");

            migrationBuilder.CreateIndex(
                name: "IX_runningAccountRecords_UserId_DateCreated",
                table: "runningAccountRecords",
                columns: new[] { "UserId", "DateCreated" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_runningAccountRecords_UserId_DateCreated",
                table: "runningAccountRecords");

            migrationBuilder.CreateIndex(
                name: "IX_runningAccountRecords_UserId",
                table: "runningAccountRecords",
                column: "UserId");
        }
    }
}
