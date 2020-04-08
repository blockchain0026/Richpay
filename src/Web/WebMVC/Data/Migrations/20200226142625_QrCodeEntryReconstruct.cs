using Microsoft.EntityFrameworkCore.Migrations;

namespace WebMVC.Data.Migrations
{
    public partial class QrCodeEntryReconstruct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PairingInfo_SpecifiedShopUsername",
                table: "qrCodeEntrys",
                newName: "SpecifiedShopUsername");

            migrationBuilder.RenameColumn(
                name: "PairingInfo_SpecifiedShopId",
                table: "qrCodeEntrys",
                newName: "SpecifiedShopId");

            migrationBuilder.RenameColumn(
                name: "PairingInfo_SpecifiedShopFullName",
                table: "qrCodeEntrys",
                newName: "SpecifiedShopFullName");

            migrationBuilder.RenameColumn(
                name: "PairingInfo_QuotaLeftToday",
                table: "qrCodeEntrys",
                newName: "QuotaLeftToday");

            migrationBuilder.RenameColumn(
                name: "PairingInfo_MinCommissionRateInThousandth",
                table: "qrCodeEntrys",
                newName: "MinCommissionRateInThousandth");

            migrationBuilder.RenameColumn(
                name: "PairingInfo_IsOnline",
                table: "qrCodeEntrys",
                newName: "IsOnline");

            migrationBuilder.RenameColumn(
                name: "PairingInfo_DateLastTraded",
                table: "qrCodeEntrys",
                newName: "DateLastTraded");

            migrationBuilder.RenameColumn(
                name: "PairingInfo_AvailableBalance",
                table: "qrCodeEntrys",
                newName: "AvailableBalance");

            migrationBuilder.AlterColumn<decimal>(
                name: "QuotaLeftToday",
                table: "qrCodeEntrys",
                type: "decimal(18,3)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,3)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MinCommissionRateInThousandth",
                table: "qrCodeEntrys",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsOnline",
                table: "qrCodeEntrys",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "AvailableBalance",
                table: "qrCodeEntrys",
                type: "decimal(18,3)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,3)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SpecifiedShopUsername",
                table: "qrCodeEntrys",
                newName: "PairingInfo_SpecifiedShopUsername");

            migrationBuilder.RenameColumn(
                name: "SpecifiedShopId",
                table: "qrCodeEntrys",
                newName: "PairingInfo_SpecifiedShopId");

            migrationBuilder.RenameColumn(
                name: "SpecifiedShopFullName",
                table: "qrCodeEntrys",
                newName: "PairingInfo_SpecifiedShopFullName");

            migrationBuilder.RenameColumn(
                name: "QuotaLeftToday",
                table: "qrCodeEntrys",
                newName: "PairingInfo_QuotaLeftToday");

            migrationBuilder.RenameColumn(
                name: "MinCommissionRateInThousandth",
                table: "qrCodeEntrys",
                newName: "PairingInfo_MinCommissionRateInThousandth");

            migrationBuilder.RenameColumn(
                name: "IsOnline",
                table: "qrCodeEntrys",
                newName: "PairingInfo_IsOnline");

            migrationBuilder.RenameColumn(
                name: "DateLastTraded",
                table: "qrCodeEntrys",
                newName: "PairingInfo_DateLastTraded");

            migrationBuilder.RenameColumn(
                name: "AvailableBalance",
                table: "qrCodeEntrys",
                newName: "PairingInfo_AvailableBalance");

            migrationBuilder.AlterColumn<decimal>(
                name: "PairingInfo_QuotaLeftToday",
                table: "qrCodeEntrys",
                type: "decimal(18,3)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,3)");

            migrationBuilder.AlterColumn<int>(
                name: "PairingInfo_MinCommissionRateInThousandth",
                table: "qrCodeEntrys",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<bool>(
                name: "PairingInfo_IsOnline",
                table: "qrCodeEntrys",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<decimal>(
                name: "PairingInfo_AvailableBalance",
                table: "qrCodeEntrys",
                type: "decimal(18,3)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,3)");
        }
    }
}
