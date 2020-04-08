using Microsoft.EntityFrameworkCore.Migrations;

namespace WebMVC.Data.Migrations
{
    public partial class WithdrawalAndDepositViewModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "depositEntrys",
                columns: table => new
                {
                    DepositId = table.Column<int>(nullable: false),
                    DepositStatus = table.Column<string>(nullable: false),
                    BalanceId = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(nullable: false),
                    Username = table.Column<string>(nullable: false),
                    FullName = table.Column<string>(nullable: false),
                    UserType = table.Column<string>(nullable: false),
                    DepositBankAccount_BankAccountId = table.Column<int>(nullable: true),
                    DepositBankAccount_BankName = table.Column<string>(nullable: true),
                    DepositBankAccount_AccountName = table.Column<string>(nullable: true),
                    DepositBankAccount_AccountNumber = table.Column<string>(nullable: true),
                    DepositBankAccount_DateCreated = table.Column<string>(nullable: true),
                    TotalAmount = table.Column<int>(nullable: false),
                    CommissionRateInThousandth = table.Column<int>(nullable: false),
                    CommissionAmount = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    ActualAmount = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    Description = table.Column<string>(nullable: true),
                    VerifiedByAdminId = table.Column<string>(nullable: true),
                    VerifiedByAdminName = table.Column<string>(nullable: true),
                    DateCreated = table.Column<string>(nullable: false),
                    DateFinished = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_depositEntrys", x => x.DepositId);
                });

            migrationBuilder.CreateTable(
                name: "withdrawalEntrys",
                columns: table => new
                {
                    WithdrawalId = table.Column<int>(nullable: false),
                    WithdrawalStatus = table.Column<string>(nullable: false),
                    BalanceId = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(nullable: false),
                    Username = table.Column<string>(nullable: false),
                    FullName = table.Column<string>(nullable: false),
                    UserType = table.Column<string>(nullable: false),
                    WithdrawalBankOption_WithdrawalBankId = table.Column<int>(nullable: true),
                    WithdrawalBankOption_BankName = table.Column<string>(nullable: true),
                    WithdrawalBankOption_DateCreated = table.Column<string>(nullable: true),
                    AccountName = table.Column<string>(nullable: false),
                    AccountNumber = table.Column<string>(nullable: false),
                    TotalAmount = table.Column<int>(nullable: false),
                    CommissionRateInThousandth = table.Column<int>(nullable: false),
                    CommissionAmount = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    ActualAmount = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    ApprovedByAdminId = table.Column<string>(nullable: true),
                    ApprovedByAdminName = table.Column<string>(nullable: true),
                    CancellationApprovedByAdminId = table.Column<string>(nullable: true),
                    CancellationApprovedByAdminName = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    DateCreated = table.Column<string>(nullable: false),
                    DateFinished = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_withdrawalEntrys", x => x.WithdrawalId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "depositEntrys");

            migrationBuilder.DropTable(
                name: "withdrawalEntrys");
        }
    }
}
