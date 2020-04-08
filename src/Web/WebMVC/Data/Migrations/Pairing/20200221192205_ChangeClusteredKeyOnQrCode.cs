using Microsoft.EntityFrameworkCore.Migrations;

namespace WebMVC.Data.Migrations.Pairing
{
    public partial class ChangeClusteredKeyOnQrCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_qrCodes",
                schema: "pairing",
                table: "qrCodes");

            migrationBuilder.DropIndex(
                name: "IX_qrCodes_PairingStatusId",
                schema: "pairing",
                table: "qrCodes");

            migrationBuilder.DropIndex(
                name: "IX_qrCodes_PaymentChannelId_PaymentSchemeId_DateLastTraded_MinCommissionRate_AvailableBalance_SpecifiedShopId_QuotaLeftToday",
                schema: "pairing",
                table: "qrCodes");

            migrationBuilder.AddPrimaryKey(
                name: "PK_qrCodes",
                schema: "pairing",
                table: "qrCodes",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_qrCodes_PaymentChannelId",
                schema: "pairing",
                table: "qrCodes",
                column: "PaymentChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_qrCodes_PairingStatusId_PaymentChannelId_PaymentSchemeId_DateLastTraded_MinCommissionRate_AvailableBalance_SpecifiedShopId_Q~",
                schema: "pairing",
                table: "qrCodes",
                columns: new[] { "PairingStatusId", "PaymentChannelId", "PaymentSchemeId", "DateLastTraded", "MinCommissionRate", "AvailableBalance", "SpecifiedShopId", "QuotaLeftToday" })
                .Annotation("SqlServer:Clustered", false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_qrCodes",
                schema: "pairing",
                table: "qrCodes");

            migrationBuilder.DropIndex(
                name: "IX_qrCodes_PaymentChannelId",
                schema: "pairing",
                table: "qrCodes");

            migrationBuilder.DropIndex(
                name: "IX_qrCodes_PairingStatusId_PaymentChannelId_PaymentSchemeId_DateLastTraded_MinCommissionRate_AvailableBalance_SpecifiedShopId_Q~",
                schema: "pairing",
                table: "qrCodes");

            migrationBuilder.AddPrimaryKey(
                name: "PK_qrCodes",
                schema: "pairing",
                table: "qrCodes",
                column: "Id")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_qrCodes_PairingStatusId",
                schema: "pairing",
                table: "qrCodes",
                column: "PairingStatusId")
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_qrCodes_PaymentChannelId_PaymentSchemeId_DateLastTraded_MinCommissionRate_AvailableBalance_SpecifiedShopId_QuotaLeftToday",
                schema: "pairing",
                table: "qrCodes",
                columns: new[] { "PaymentChannelId", "PaymentSchemeId", "DateLastTraded", "MinCommissionRate", "AvailableBalance", "SpecifiedShopId", "QuotaLeftToday" })
                .Annotation("SqlServer:Clustered", false);
        }
    }
}
