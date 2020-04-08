using Microsoft.EntityFrameworkCore.Migrations;

namespace WebMVC.Data.Migrations
{
    public partial class BankbookRecordsViewModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "bankbookrecordseq",
                incrementBy: 10);

            migrationBuilder.CreateTable(
                name: "bankbookRecords",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(nullable: false),
                    BalanceId = table.Column<int>(nullable: false),
                    DateOccurred = table.Column<string>(nullable: false),
                    Type = table.Column<string>(nullable: false),
                    BalanceBefore = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    AmountChanged = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    BalanceAfter = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    TrackingId = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bankbookRecords", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "bankbookRecords");

            migrationBuilder.DropSequence(
                name: "bankbookrecordseq");
        }
    }
}
