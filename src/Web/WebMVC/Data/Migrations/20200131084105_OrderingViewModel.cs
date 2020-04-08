using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebMVC.Data.Migrations
{
    public partial class OrderingViewModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FourthPartyId",
                table: "shopGatewayEntrys");

            migrationBuilder.DropColumn(
                name: "GatewayNumber",
                table: "shopGatewayEntrys");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "shopGatewayEntrys");

            migrationBuilder.CreateSequence(
                name: "runningaccountrecordseq",
                incrementBy: 10);

            migrationBuilder.AddColumn<int>(
                name: "FourthPartyGatewayId",
                table: "shopGatewayEntrys",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "orderEntrys",
                columns: table => new
                {
                    OrderId = table.Column<int>(nullable: false),
                    TrackingNumber = table.Column<string>(nullable: false),
                    OrderType = table.Column<string>(nullable: false),
                    OrderStatus = table.Column<string>(nullable: false),
                    OrderStatusDescription = table.Column<string>(nullable: true),
                    IsTestOrder = table.Column<bool>(nullable: false),
                    IsExpired = table.Column<bool>(nullable: false),
                    ShopId = table.Column<string>(nullable: false),
                    ShopUserName = table.Column<string>(nullable: false),
                    ShopFullName = table.Column<string>(nullable: false),
                    ShopOrderId = table.Column<string>(nullable: false),
                    RateRebateShop = table.Column<decimal>(nullable: false),
                    RateRebateFinal = table.Column<decimal>(nullable: false),
                    ToppestTradingRate = table.Column<decimal>(nullable: false),
                    OrderPaymentChannel = table.Column<string>(nullable: false),
                    OrderPaymentScheme = table.Column<string>(nullable: false),
                    IpAddress = table.Column<string>(nullable: true),
                    Device = table.Column<string>(nullable: true),
                    Location = table.Column<string>(nullable: true),
                    TraderId = table.Column<string>(nullable: true),
                    TraderUserName = table.Column<string>(nullable: true),
                    TraderFullName = table.Column<string>(nullable: true),
                    QrCodeId = table.Column<int>(nullable: true),
                    FourthPartyId = table.Column<string>(nullable: true),
                    FourthPartyName = table.Column<string>(nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    AmountPaid = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    ShopUserCommissionRealized = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    TradingUserCommissionRealized = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DatePaired = table.Column<string>(nullable: true),
                    DatePaymentRecieved = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orderEntrys", x => x.OrderId);
                });

            migrationBuilder.CreateTable(
                name: "runningAccountRecords",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(nullable: false),
                    UserFullName = table.Column<string>(nullable: false),
                    DownlineUserId = table.Column<string>(nullable: false),
                    DownlineUserName = table.Column<string>(nullable: false),
                    DownlineFullName = table.Column<string>(nullable: false),
                    OrderTrackingNumber = table.Column<string>(nullable: false),
                    Status = table.Column<string>(nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    CommissionAmount = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    DistributionId = table.Column<int>(nullable: true),
                    DistributedAmount = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    ShopId = table.Column<string>(nullable: false),
                    ShopUserName = table.Column<string>(nullable: false),
                    ShopFullName = table.Column<string>(nullable: false),
                    TraderId = table.Column<string>(nullable: false),
                    TraderUserName = table.Column<string>(nullable: false),
                    TraderFullName = table.Column<string>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_runningAccountRecords", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "orderEntrys");

            migrationBuilder.DropTable(
                name: "runningAccountRecords");

            migrationBuilder.DropSequence(
                name: "runningaccountrecordseq");

            migrationBuilder.DropColumn(
                name: "FourthPartyGatewayId",
                table: "shopGatewayEntrys");

            migrationBuilder.AddColumn<int>(
                name: "FourthPartyId",
                table: "shopGatewayEntrys",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GatewayNumber",
                table: "shopGatewayEntrys",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "shopGatewayEntrys",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
