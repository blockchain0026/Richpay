using Microsoft.EntityFrameworkCore.Migrations;

namespace WebMVC.Data.Migrations
{
    public partial class ShopViewModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "shops",
                schema: "application",
                columns: table => new
                {
                    ShopId = table.Column<string>(nullable: false),
                    Username = table.Column<string>(nullable: false),
                    FullName = table.Column<string>(nullable: false),
                    SiteAddress = table.Column<string>(nullable: false),
                    PhoneNumber = table.Column<string>(nullable: false),
                    Email = table.Column<string>(nullable: false),
                    Wechat = table.Column<string>(nullable: true),
                    QQ = table.Column<string>(nullable: true),
                    UplineUserId = table.Column<string>(nullable: true),
                    UplineUserName = table.Column<string>(nullable: true),
                    UplineFullName = table.Column<string>(nullable: true),
                    Balance_AmountAvailable = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    Balance_AmountFrozen = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    Balance_WithdrawalLimit_DailyAmountLimit = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    Balance_WithdrawalLimit_DailyFrequencyLimit = table.Column<int>(nullable: true),
                    Balance_WithdrawalLimit_EachAmountUpperLimit = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    Balance_WithdrawalLimit_EachAmountLowerLimit = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    Balance_WithdrawalCommissionRateInThousandth = table.Column<int>(nullable: true),
                    RebateCommission_RateRebateAlipayInThousandth = table.Column<int>(nullable: true),
                    RebateCommission_RateRebateWechatInThousandth = table.Column<int>(nullable: true),
                    IsOpen = table.Column<bool>(nullable: false),
                    IsEnabled = table.Column<bool>(nullable: false),
                    IsReviewed = table.Column<bool>(nullable: false),
                    LastLoginIP = table.Column<string>(nullable: true),
                    DateLastLoggedIn = table.Column<string>(nullable: true),
                    DateCreated = table.Column<string>(nullable: false),
                    DateLastTrade = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shops", x => x.ShopId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "shops",
                schema: "application");
        }
    }
}
