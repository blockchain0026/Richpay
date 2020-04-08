using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebMVC.Data.Migrations.Ordering
{
    public partial class InitailCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "ordering");

            migrationBuilder.CreateSequence(
                name: "balancewithdrawalseq",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "orderseq",
                schema: "ordering",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "shopapiseq",
                schema: "ordering",
                incrementBy: 10);

            migrationBuilder.CreateTable(
                name: "orderPaymentChannel",
                schema: "ordering",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false, defaultValue: 1),
                    Name = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orderPaymentChannel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "orderPaymentScheme",
                schema: "ordering",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false, defaultValue: 1),
                    Name = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orderPaymentScheme", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "orderstatus",
                schema: "ordering",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false, defaultValue: 1),
                    Name = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orderstatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ordertype",
                schema: "ordering",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false, defaultValue: 1),
                    Name = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ordertype", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "requests",
                schema: "ordering",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Time = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_requests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "shopApis",
                schema: "ordering",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    ShopId = table.Column<string>(nullable: false),
                    ApiKey = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shopApis", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "orders",
                schema: "ordering",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    TrackingNumber = table.Column<string>(nullable: true),
                    OrderType = table.Column<int>(nullable: false),
                    OrderStatus = table.Column<int>(nullable: false),
                    OrderStatusDescription = table.Column<string>(nullable: true),
                    ExpirationTimeInSeconds = table.Column<int>(nullable: false),
                    IsTestOrder = table.Column<bool>(nullable: false),
                    IsExpired = table.Column<bool>(nullable: false),
                    ShopInfo_ShopId = table.Column<string>(nullable: true),
                    ShopInfo_ShopOrderId = table.Column<string>(nullable: true),
                    ShopInfo_ShopReturnUrl = table.Column<string>(nullable: true),
                    ShopInfo_ShopOkReturnUrl = table.Column<string>(nullable: true),
                    ShopInfo_RateRebateShop = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    ShopInfo_RateRebateFinal = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    OrderPaymentChannel = table.Column<int>(nullable: false),
                    OrderPaymentScheme = table.Column<int>(nullable: false),
                    AlipayPagePreference_SecondsBeforePayment = table.Column<int>(nullable: true),
                    AlipayPagePreference_IsAmountUnchangeable = table.Column<bool>(nullable: true),
                    AlipayPagePreference_IsAccountUnchangeable = table.Column<bool>(nullable: true),
                    AlipayPagePreference_IsH5RedirectByScanEnabled = table.Column<bool>(nullable: true),
                    AlipayPagePreference_IsH5RedirectByClickEnabled = table.Column<bool>(nullable: true),
                    AlipayPagePreference_IsH5RedirectByPickingPhotoEnabled = table.Column<bool>(nullable: true),
                    PayerInfo_IpAddress = table.Column<string>(nullable: true),
                    PayerInfo_Device = table.Column<string>(nullable: true),
                    PayerInfo_Location = table.Column<string>(nullable: true),
                    PayeeInfo_TraderId = table.Column<string>(nullable: true),
                    PayeeInfo_QrCodeId = table.Column<int>(nullable: true),
                    PayeeInfo_FourthPartyId = table.Column<string>(nullable: true),
                    PayeeInfo_FourthPartyName = table.Column<string>(nullable: true),
                    PayeeInfo_FourthPartyOrderPaymentUrl = table.Column<string>(nullable: true),
                    PayeeInfo_FourthPartyOrderNumber = table.Column<string>(nullable: true),
                    PayeeInfo_ToppestTradingRate = table.Column<decimal>(type: "decimal(4,3)", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    AmountPaid = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    CommissionRealized = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DatePaired = table.Column<DateTime>(nullable: true),
                    DatePaymentRecieved = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_orders_orderPaymentChannel_OrderPaymentChannel",
                        column: x => x.OrderPaymentChannel,
                        principalSchema: "ordering",
                        principalTable: "orderPaymentChannel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_orders_orderPaymentScheme_OrderPaymentScheme",
                        column: x => x.OrderPaymentScheme,
                        principalSchema: "ordering",
                        principalTable: "orderPaymentScheme",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_orders_orderstatus_OrderStatus",
                        column: x => x.OrderStatus,
                        principalSchema: "ordering",
                        principalTable: "orderstatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_orders_ordertype_OrderType",
                        column: x => x.OrderType,
                        principalSchema: "ordering",
                        principalTable: "ordertype",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ipWhitelists",
                schema: "ordering",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    ShopApiId = table.Column<int>(nullable: true),
                    Address = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ipWhitelists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ipWhitelists_shopApis_ShopApiId",
                        column: x => x.ShopApiId,
                        principalSchema: "ordering",
                        principalTable: "shopApis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ipWhitelists_ShopApiId",
                schema: "ordering",
                table: "ipWhitelists",
                column: "ShopApiId");

            migrationBuilder.CreateIndex(
                name: "IX_orders_OrderPaymentChannel",
                schema: "ordering",
                table: "orders",
                column: "OrderPaymentChannel");

            migrationBuilder.CreateIndex(
                name: "IX_orders_OrderPaymentScheme",
                schema: "ordering",
                table: "orders",
                column: "OrderPaymentScheme");

            migrationBuilder.CreateIndex(
                name: "IX_orders_OrderStatus",
                schema: "ordering",
                table: "orders",
                column: "OrderStatus");

            migrationBuilder.CreateIndex(
                name: "IX_orders_OrderType",
                schema: "ordering",
                table: "orders",
                column: "OrderType");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ipWhitelists",
                schema: "ordering");

            migrationBuilder.DropTable(
                name: "orders",
                schema: "ordering");

            migrationBuilder.DropTable(
                name: "requests",
                schema: "ordering");

            migrationBuilder.DropTable(
                name: "shopApis",
                schema: "ordering");

            migrationBuilder.DropTable(
                name: "orderPaymentChannel",
                schema: "ordering");

            migrationBuilder.DropTable(
                name: "orderPaymentScheme",
                schema: "ordering");

            migrationBuilder.DropTable(
                name: "orderstatus",
                schema: "ordering");

            migrationBuilder.DropTable(
                name: "ordertype",
                schema: "ordering");

            migrationBuilder.DropSequence(
                name: "balancewithdrawalseq");

            migrationBuilder.DropSequence(
                name: "orderseq",
                schema: "ordering");

            migrationBuilder.DropSequence(
                name: "shopapiseq",
                schema: "ordering");
        }
    }
}
