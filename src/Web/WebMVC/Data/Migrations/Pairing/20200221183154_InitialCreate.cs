using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebMVC.Data.Migrations.Pairing
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "pairing");

            migrationBuilder.CreateSequence(
                name: "orderamountoptionseq",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "qrcodeorderseq",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "clouddeviceseq",
                schema: "pairing",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "fourthpartygatewayseq",
                schema: "pairing",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "qrcodeseq",
                schema: "pairing",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "shopgatewayseq",
                schema: "pairing",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "shopsettingsseq",
                schema: "pairing",
                incrementBy: 10);

            migrationBuilder.CreateTable(
                name: "cloudDeviceStatus",
                schema: "pairing",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false, defaultValue: 1),
                    Name = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cloudDeviceStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "pairingStatus",
                schema: "pairing",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false, defaultValue: 1),
                    Name = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pairingStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "paymentChannel",
                schema: "pairing",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false, defaultValue: 1),
                    Name = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_paymentChannel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "paymentScheme",
                schema: "pairing",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false, defaultValue: 1),
                    Name = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_paymentScheme", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "qrCodeOrders",
                schema: "pairing",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    OrderTrackingNumber = table.Column<string>(nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateFinished = table.Column<DateTime>(nullable: true),
                    IsFailed = table.Column<bool>(nullable: false),
                    IsSuccess = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_qrCodeOrders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "qrCodeStatus",
                schema: "pairing",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false, defaultValue: 1),
                    Name = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_qrCodeStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "qrCodeType",
                schema: "pairing",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false, defaultValue: 1),
                    Name = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_qrCodeType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "requests",
                schema: "pairing",
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
                name: "shopGatewayType",
                schema: "pairing",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false, defaultValue: 1),
                    Name = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shopGatewayType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "shopSettingss",
                schema: "pairing",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    ShopId = table.Column<string>(nullable: false),
                    IsOpen = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shopSettingss", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "cloudDevices",
                schema: "pairing",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(nullable: false),
                    CloudDeviceStatusId1 = table.Column<int>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    Number = table.Column<string>(nullable: false),
                    LoginUsername = table.Column<string>(nullable: false),
                    LoginPassword = table.Column<string>(nullable: false),
                    ApiKey = table.Column<string>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    CloudDeviceStatusId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cloudDevices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_cloudDevices_cloudDeviceStatus_CloudDeviceStatusId1",
                        column: x => x.CloudDeviceStatusId1,
                        principalSchema: "pairing",
                        principalTable: "cloudDeviceStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "fourthPartyGateways",
                schema: "pairing",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(nullable: false),
                    PaymentChannelId1 = table.Column<int>(nullable: true),
                    PaymentSchemeId1 = table.Column<int>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    OpenFrom = table.Column<TimeSpan>(nullable: false),
                    OpenTo = table.Column<TimeSpan>(nullable: false),
                    IsEnabled = table.Column<bool>(nullable: false),
                    PaymentChannelId = table.Column<int>(nullable: false),
                    PaymentSchemeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fourthPartyGateways", x => x.Id);
                    table.ForeignKey(
                        name: "FK_fourthPartyGateways_paymentChannel_PaymentChannelId1",
                        column: x => x.PaymentChannelId1,
                        principalSchema: "pairing",
                        principalTable: "paymentChannel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_fourthPartyGateways_paymentScheme_PaymentSchemeId1",
                        column: x => x.PaymentSchemeId1,
                        principalSchema: "pairing",
                        principalTable: "paymentScheme",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_fourthPartyGateways_paymentChannel_PaymentChannelId",
                        column: x => x.PaymentChannelId,
                        principalSchema: "pairing",
                        principalTable: "paymentChannel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_fourthPartyGateways_paymentScheme_PaymentSchemeId",
                        column: x => x.PaymentSchemeId,
                        principalSchema: "pairing",
                        principalTable: "paymentScheme",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "orderAmountOptions",
                schema: "pairing",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    ShopId = table.Column<string>(nullable: false),
                    ShopSettingsId = table.Column<int>(nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orderAmountOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_orderAmountOptions_shopSettingss_ShopSettingsId",
                        column: x => x.ShopSettingsId,
                        principalSchema: "pairing",
                        principalTable: "shopSettingss",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "qrCodes",
                schema: "pairing",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(nullable: false),
                    CloudDeviceId = table.Column<int>(nullable: true),
                    QrCodeTypeId = table.Column<int>(nullable: false),
                    PaymentChannelId = table.Column<int>(nullable: false),
                    PaymentSchemeId = table.Column<int>(nullable: false),
                    QrCodeStatusId = table.Column<int>(nullable: false),
                    PairingStatusId = table.Column<int>(nullable: false),
                    CodeStatusDiscription = table.Column<string>(nullable: true),
                    PairingStatusDescription = table.Column<string>(nullable: true),
                    QrCodeSettings_AutoPairingBySuccessRate = table.Column<bool>(nullable: true),
                    QrCodeSettings_AutoPairingByQuotaLeft = table.Column<bool>(nullable: true),
                    QrCodeSettings_AutoPairingByBusinessHours = table.Column<bool>(nullable: true),
                    QrCodeSettings_AutoPairingByCurrentConsecutiveFailures = table.Column<bool>(nullable: true),
                    QrCodeSettings_AutoPairngByAvailableBalance = table.Column<bool>(nullable: true),
                    QrCodeSettings_SuccessRateThreshold = table.Column<decimal>(type: "decimal(3,2)", nullable: true),
                    QrCodeSettings_SuccessRateMinOrders = table.Column<int>(nullable: true),
                    QrCodeSettings_QuotaLeftThreshold = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    QrCodeSettings_CurrentConsecutiveFailuresThreshold = table.Column<int>(nullable: true),
                    QrCodeSettings_AvailableBalanceThreshold = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    DailyAmountLimit = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    OrderAmountUpperLimit = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    OrderAmountLowerLimit = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    FullName = table.Column<string>(nullable: false),
                    IsApproved = table.Column<bool>(nullable: false),
                    ApprovedBy_AdminId = table.Column<string>(nullable: true),
                    ApprovedBy_Name = table.Column<string>(nullable: true),
                    BarcodeDataForManual_QrCodeUrl = table.Column<string>(nullable: true),
                    BarcodeDataForManual_Amount = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    BarcodeDataForAuto_CloudDeviceUsername = table.Column<string>(nullable: true),
                    BarcodeDataForAuto_CloudDevicePassword = table.Column<string>(nullable: true),
                    BarcodeDataForAuto_CloudDeviceNumber = table.Column<string>(nullable: true),
                    MerchantData_AppId = table.Column<string>(nullable: true),
                    MerchantData_AlipayPublicKey = table.Column<string>(nullable: true),
                    MerchantData_WechatApiCertificate = table.Column<string>(nullable: true),
                    MerchantData_PrivateKey = table.Column<string>(nullable: true),
                    MerchantData_MerchantId = table.Column<string>(nullable: true),
                    TransactionData_UserId = table.Column<string>(nullable: true),
                    BankData_BankName = table.Column<string>(nullable: true),
                    BankData_BankMark = table.Column<string>(nullable: true),
                    BankData_AccountName = table.Column<string>(nullable: true),
                    BankData_CardNumber = table.Column<string>(nullable: true),
                    PairingData_TotalSuccess = table.Column<int>(nullable: true),
                    PairingData_TotalFailures = table.Column<int>(nullable: true),
                    PairingData_HighestConsecutiveSuccess = table.Column<int>(nullable: true),
                    PairingData_HighestConsecutiveFailures = table.Column<int>(nullable: true),
                    PairingData_CurrentConsecutiveSuccess = table.Column<int>(nullable: true),
                    PairingData_CurrentConsecutiveFailures = table.Column<int>(nullable: true),
                    PairingData_SuccessRate = table.Column<decimal>(type: "decimal(3,2)", nullable: true),
                    IsOnline = table.Column<bool>(nullable: false),
                    MinCommissionRate = table.Column<decimal>(type: "decimal(4,3)", nullable: false),
                    AvailableBalance = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    SpecifiedShopId = table.Column<string>(unicode: false, nullable: true),
                    QuotaLeftToday = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    DateLastTraded = table.Column<DateTime>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_qrCodes", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_qrCodes_cloudDevices_CloudDeviceId",
                        column: x => x.CloudDeviceId,
                        principalSchema: "pairing",
                        principalTable: "cloudDevices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_qrCodes_pairingStatus_PairingStatusId",
                        column: x => x.PairingStatusId,
                        principalSchema: "pairing",
                        principalTable: "pairingStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_qrCodes_paymentChannel_PaymentChannelId",
                        column: x => x.PaymentChannelId,
                        principalSchema: "pairing",
                        principalTable: "paymentChannel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_qrCodes_paymentScheme_PaymentSchemeId",
                        column: x => x.PaymentSchemeId,
                        principalSchema: "pairing",
                        principalTable: "paymentScheme",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_qrCodes_qrCodeStatus_QrCodeStatusId",
                        column: x => x.QrCodeStatusId,
                        principalSchema: "pairing",
                        principalTable: "qrCodeStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_qrCodes_qrCodeType_QrCodeTypeId",
                        column: x => x.QrCodeTypeId,
                        principalSchema: "pairing",
                        principalTable: "qrCodeType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "shopGateways",
                schema: "pairing",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    ShopId = table.Column<string>(nullable: false),
                    ShopGatewayTypeId1 = table.Column<int>(nullable: true),
                    PaymentChannelId1 = table.Column<int>(nullable: true),
                    PaymentSchemeId1 = table.Column<int>(nullable: true),
                    AlipayPreference_SecondsBeforePayment = table.Column<int>(nullable: true),
                    AlipayPreference_IsAmountUnchangeable = table.Column<bool>(nullable: true),
                    AlipayPreference_IsAccountUnchangeable = table.Column<bool>(nullable: true),
                    AlipayPreference_IsH5RedirectByScanEnabled = table.Column<bool>(nullable: true),
                    AlipayPreference_IsH5RedirectByClickEnabled = table.Column<bool>(nullable: true),
                    AlipayPreference_IsH5RedirectByPickingPhotoEnabled = table.Column<bool>(nullable: true),
                    FourthPartyGatewayId = table.Column<int>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    PaymentChannelId = table.Column<int>(nullable: false),
                    PaymentSchemeId = table.Column<int>(nullable: false),
                    ShopGatewayTypeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shopGateways", x => x.Id);
                    table.ForeignKey(
                        name: "FK_shopGateways_fourthPartyGateways_FourthPartyGatewayId",
                        column: x => x.FourthPartyGatewayId,
                        principalSchema: "pairing",
                        principalTable: "fourthPartyGateways",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_shopGateways_paymentChannel_PaymentChannelId1",
                        column: x => x.PaymentChannelId1,
                        principalSchema: "pairing",
                        principalTable: "paymentChannel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_shopGateways_paymentScheme_PaymentSchemeId1",
                        column: x => x.PaymentSchemeId1,
                        principalSchema: "pairing",
                        principalTable: "paymentScheme",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_shopGateways_shopGatewayType_ShopGatewayTypeId1",
                        column: x => x.ShopGatewayTypeId1,
                        principalSchema: "pairing",
                        principalTable: "shopGatewayType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_shopGateways_paymentChannel_PaymentChannelId",
                        column: x => x.PaymentChannelId,
                        principalSchema: "pairing",
                        principalTable: "paymentChannel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_shopGateways_paymentScheme_PaymentSchemeId",
                        column: x => x.PaymentSchemeId,
                        principalSchema: "pairing",
                        principalTable: "paymentScheme",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_shopGateways_shopGatewayType_ShopGatewayTypeId",
                        column: x => x.ShopGatewayTypeId,
                        principalSchema: "pairing",
                        principalTable: "shopGatewayType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_cloudDevices_CloudDeviceStatusId1",
                schema: "pairing",
                table: "cloudDevices",
                column: "CloudDeviceStatusId1");

            migrationBuilder.CreateIndex(
                name: "IX_fourthPartyGateways_PaymentChannelId1",
                schema: "pairing",
                table: "fourthPartyGateways",
                column: "PaymentChannelId1");

            migrationBuilder.CreateIndex(
                name: "IX_fourthPartyGateways_PaymentSchemeId1",
                schema: "pairing",
                table: "fourthPartyGateways",
                column: "PaymentSchemeId1");

            migrationBuilder.CreateIndex(
                name: "IX_fourthPartyGateways_PaymentChannelId",
                schema: "pairing",
                table: "fourthPartyGateways",
                column: "PaymentChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_fourthPartyGateways_PaymentSchemeId",
                schema: "pairing",
                table: "fourthPartyGateways",
                column: "PaymentSchemeId");

            migrationBuilder.CreateIndex(
                name: "IX_orderAmountOptions_ShopSettingsId",
                schema: "pairing",
                table: "orderAmountOptions",
                column: "ShopSettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_qrCodes_CloudDeviceId",
                schema: "pairing",
                table: "qrCodes",
                column: "CloudDeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_qrCodes_PairingStatusId",
                schema: "pairing",
                table: "qrCodes",
                column: "PairingStatusId")
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_qrCodes_PaymentSchemeId",
                schema: "pairing",
                table: "qrCodes",
                column: "PaymentSchemeId");

            migrationBuilder.CreateIndex(
                name: "IX_qrCodes_QrCodeStatusId",
                schema: "pairing",
                table: "qrCodes",
                column: "QrCodeStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_qrCodes_QrCodeTypeId",
                schema: "pairing",
                table: "qrCodes",
                column: "QrCodeTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_qrCodes_PaymentChannelId_PaymentSchemeId_DateLastTraded_MinCommissionRate_AvailableBalance_SpecifiedShopId_QuotaLeftToday",
                schema: "pairing",
                table: "qrCodes",
                columns: new[] { "PaymentChannelId", "PaymentSchemeId", "DateLastTraded", "MinCommissionRate", "AvailableBalance", "SpecifiedShopId", "QuotaLeftToday" })
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_shopGateways_FourthPartyGatewayId",
                schema: "pairing",
                table: "shopGateways",
                column: "FourthPartyGatewayId");

            migrationBuilder.CreateIndex(
                name: "IX_shopGateways_PaymentChannelId1",
                schema: "pairing",
                table: "shopGateways",
                column: "PaymentChannelId1");

            migrationBuilder.CreateIndex(
                name: "IX_shopGateways_PaymentSchemeId1",
                schema: "pairing",
                table: "shopGateways",
                column: "PaymentSchemeId1");

            migrationBuilder.CreateIndex(
                name: "IX_shopGateways_ShopGatewayTypeId1",
                schema: "pairing",
                table: "shopGateways",
                column: "ShopGatewayTypeId1");

            migrationBuilder.CreateIndex(
                name: "IX_shopGateways_PaymentChannelId",
                schema: "pairing",
                table: "shopGateways",
                column: "PaymentChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_shopGateways_PaymentSchemeId",
                schema: "pairing",
                table: "shopGateways",
                column: "PaymentSchemeId");

            migrationBuilder.CreateIndex(
                name: "IX_shopGateways_ShopGatewayTypeId",
                schema: "pairing",
                table: "shopGateways",
                column: "ShopGatewayTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "orderAmountOptions",
                schema: "pairing");

            migrationBuilder.DropTable(
                name: "qrCodeOrders",
                schema: "pairing");

            migrationBuilder.DropTable(
                name: "qrCodes",
                schema: "pairing");

            migrationBuilder.DropTable(
                name: "requests",
                schema: "pairing");

            migrationBuilder.DropTable(
                name: "shopGateways",
                schema: "pairing");

            migrationBuilder.DropTable(
                name: "shopSettingss",
                schema: "pairing");

            migrationBuilder.DropTable(
                name: "cloudDevices",
                schema: "pairing");

            migrationBuilder.DropTable(
                name: "pairingStatus",
                schema: "pairing");

            migrationBuilder.DropTable(
                name: "qrCodeStatus",
                schema: "pairing");

            migrationBuilder.DropTable(
                name: "qrCodeType",
                schema: "pairing");

            migrationBuilder.DropTable(
                name: "fourthPartyGateways",
                schema: "pairing");

            migrationBuilder.DropTable(
                name: "shopGatewayType",
                schema: "pairing");

            migrationBuilder.DropTable(
                name: "cloudDeviceStatus",
                schema: "pairing");

            migrationBuilder.DropTable(
                name: "paymentChannel",
                schema: "pairing");

            migrationBuilder.DropTable(
                name: "paymentScheme",
                schema: "pairing");

            migrationBuilder.DropSequence(
                name: "orderamountoptionseq");

            migrationBuilder.DropSequence(
                name: "qrcodeorderseq");

            migrationBuilder.DropSequence(
                name: "clouddeviceseq",
                schema: "pairing");

            migrationBuilder.DropSequence(
                name: "fourthpartygatewayseq",
                schema: "pairing");

            migrationBuilder.DropSequence(
                name: "qrcodeseq",
                schema: "pairing");

            migrationBuilder.DropSequence(
                name: "shopgatewayseq",
                schema: "pairing");

            migrationBuilder.DropSequence(
                name: "shopsettingsseq",
                schema: "pairing");
        }
    }
}
