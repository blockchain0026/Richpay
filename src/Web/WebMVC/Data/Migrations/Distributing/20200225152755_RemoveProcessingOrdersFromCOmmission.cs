using Microsoft.EntityFrameworkCore.Migrations;

namespace WebMVC.Data.Migrations.Distributing
{
    public partial class RemoveProcessingOrdersFromCOmmission : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_processingOrders_commissions_CommissionId",
                schema: "distributing",
                table: "processingOrders");

            migrationBuilder.DropIndex(
                name: "IX_processingOrders_CommissionId",
                schema: "distributing",
                table: "processingOrders");

            migrationBuilder.DropColumn(
                name: "CommissionId",
                schema: "distributing",
                table: "processingOrders");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CommissionId",
                schema: "distributing",
                table: "processingOrders",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_processingOrders_CommissionId",
                schema: "distributing",
                table: "processingOrders",
                column: "CommissionId");

            migrationBuilder.AddForeignKey(
                name: "FK_processingOrders_commissions_CommissionId",
                schema: "distributing",
                table: "processingOrders",
                column: "CommissionId",
                principalSchema: "distributing",
                principalTable: "commissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
