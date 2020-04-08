using Microsoft.EntityFrameworkCore.Migrations;

namespace WebMVC.Data.Migrations
{
    public partial class TraderAgentsViewModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "application");

            migrationBuilder.CreateTable(
                name: "traderAgents",
                schema: "application",
                columns: table => new
                {
                    TraderAgentId = table.Column<string>(nullable: false),
                    Username = table.Column<string>(nullable: false),
                    FullName = table.Column<string>(nullable: false),
                    Nickname = table.Column<string>(nullable: false),
                    PhoneNumber = table.Column<string>(nullable: false),
                    Email = table.Column<string>(nullable: false),
                    Wechat = table.Column<string>(nullable: true),
                    QQ = table.Column<string>(nullable: true),
                    UplineUserId = table.Column<string>(nullable: true),
                    UplineUserName = table.Column<string>(nullable: true),
                    UplineFullName = table.Column<string>(nullable: true),
                    Balance_UserId = table.Column<string>(nullable: true),
                    Balance_AmountAvailable = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    Balance_AmountFrozen = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    Balance_WithdrawalLimit_DailyAmountLimit = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    Balance_WithdrawalLimit_DailyFrequencyLimit = table.Column<int>(nullable: true),
                    Balance_WithdrawalLimit_EachAmountUpperLimit = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    Balance_WithdrawalLimit_EachAmountLowerLimit = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    Balance_WithdrawalCommissionRateInThousandth = table.Column<int>(nullable: true),
                    Balance_DepositCommissionRateInThousandth = table.Column<int>(nullable: true),
                    TradingCommission_RateAlipayInThousandth = table.Column<int>(nullable: true),
                    TradingCommission_RateWechatInThousandth = table.Column<int>(nullable: true),
                    IsEnabled = table.Column<bool>(nullable: false),
                    IsReviewed = table.Column<bool>(nullable: false),
                    HasGrantRight = table.Column<bool>(nullable: false),
                    LastLoginIP = table.Column<string>(nullable: true),
                    DateLastLoggedIn = table.Column<string>(nullable: true),
                    DateCreated = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_traderAgents", x => x.TraderAgentId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "traderAgents",
                schema: "application");
        }
    }
}
