using Microsoft.EntityFrameworkCore.Migrations;

namespace WebMVC.Data.Migrations
{
    public partial class PairingViewModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "qrCodeEntrys",
                columns: table => new
                {
                    QrCodeId = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(nullable: false),
                    Username = table.Column<string>(nullable: false),
                    UserFullName = table.Column<string>(nullable: false),
                    UplineUserId = table.Column<string>(nullable: false),
                    UplineUserName = table.Column<string>(nullable: false),
                    UplineFullName = table.Column<string>(nullable: false),
                    CloudDeviceId = table.Column<int>(nullable: true),
                    QrCodeType = table.Column<string>(nullable: false),
                    PaymentChannel = table.Column<string>(nullable: false),
                    PaymentScheme = table.Column<string>(nullable: false),
                    QrCodeStatus = table.Column<string>(nullable: false),
                    PairingStatus = table.Column<string>(nullable: false),
                    CodeStatusDescription = table.Column<string>(nullable: false),
                    PairingStatusDescription = table.Column<string>(nullable: false),
                    QrCodeEntrySetting_AutoPairingBySuccessRate = table.Column<bool>(nullable: true),
                    QrCodeEntrySetting_AutoPairingByQuotaLeft = table.Column<bool>(nullable: true),
                    QrCodeEntrySetting_AutoPairingByBusinessHours = table.Column<bool>(nullable: true),
                    QrCodeEntrySetting_AutoPairingByCurrentConsecutiveFailures = table.Column<bool>(nullable: true),
                    QrCodeEntrySetting_AutoPairngByAvailableBalance = table.Column<bool>(nullable: true),
                    QrCodeEntrySetting_SuccessRateThresholdInHundredth = table.Column<int>(nullable: true),
                    QrCodeEntrySetting_SuccessRateMinOrders = table.Column<int>(nullable: true),
                    QrCodeEntrySetting_QuotaLeftThreshold = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    QrCodeEntrySetting_CurrentConsecutiveFailuresThreshold = table.Column<int>(nullable: true),
                    QrCodeEntrySetting_AvailableBalanceThreshold = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    DailyAmountLimit = table.Column<int>(nullable: false),
                    OrderAmountUpperLimit = table.Column<int>(nullable: false),
                    OrderAmountLowerLimit = table.Column<int>(nullable: false),
                    FullName = table.Column<string>(nullable: false),
                    IsApproved = table.Column<bool>(nullable: false),
                    ApprovedByAdminId = table.Column<string>(nullable: true),
                    ApprovedByAdminName = table.Column<string>(nullable: true),
                    PairingInfo_IsOnline = table.Column<bool>(nullable: true),
                    PairingInfo_MinCommissionRateInThousandth = table.Column<int>(nullable: true),
                    PairingInfo_AvailableBalance = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    PairingInfo_SpecifiedShopId = table.Column<string>(nullable: true),
                    PairingInfo_SpecifiedShopUsername = table.Column<string>(nullable: true),
                    PairingInfo_SpecifiedShopFullName = table.Column<string>(nullable: true),
                    PairingInfo_QuotaLeftToday = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    PairingInfo_DateLastTraded = table.Column<string>(nullable: true),
                    PairingInfo_TotalSuccess = table.Column<int>(nullable: true),
                    PairingInfo_TotalFailures = table.Column<int>(nullable: true),
                    PairingInfo_HighestConsecutiveSuccess = table.Column<int>(nullable: true),
                    PairingInfo_HighestConsecutiveFailures = table.Column<int>(nullable: true),
                    PairingInfo_CurrentConsecutiveSuccess = table.Column<int>(nullable: true),
                    PairingInfo_CurrentConsecutiveFailures = table.Column<int>(nullable: true),
                    PairingInfo_SuccessRateInPercent = table.Column<int>(nullable: true),
                    DateCreated = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_qrCodeEntrys", x => x.QrCodeId);
                });

            migrationBuilder.CreateTable(
                name: "shopGatewayEntrys",
                columns: table => new
                {
                    ShopGatewayId = table.Column<int>(nullable: false),
                    ShopId = table.Column<string>(nullable: false),
                    ShopGatewayType = table.Column<string>(nullable: false),
                    PaymentChannel = table.Column<string>(nullable: false),
                    PaymentScheme = table.Column<string>(nullable: false),
                    GatewayNumber = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    FourthPartyId = table.Column<int>(nullable: true),
                    AlipayPreferenceInfo_SecondsBeforePayment = table.Column<int>(nullable: true),
                    AlipayPreferenceInfo_IsAmountUnchangeable = table.Column<bool>(nullable: true),
                    AlipayPreferenceInfo_IsAccountUnchangeable = table.Column<bool>(nullable: true),
                    AlipayPreferenceInfo_IsH5RedirectByScanEnabled = table.Column<bool>(nullable: true),
                    AlipayPreferenceInfo_IsH5RedirectByClickEnabled = table.Column<bool>(nullable: true),
                    AlipayPreferenceInfo_IsH5RedirectByPickingPhotoEnabled = table.Column<bool>(nullable: true),
                    DateCreated = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shopGatewayEntrys", x => x.ShopGatewayId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "qrCodeEntrys");

            migrationBuilder.DropTable(
                name: "shopGatewayEntrys");
        }
    }
}
