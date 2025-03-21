using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalDomain.Migrations
{
    /// <inheritdoc />
    public partial class addReceipt3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_appointment_ReceiptModel_ReceiptNavigationId",
                table: "appointment");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptRecord_ReceiptModel_ReceiptNavigationId",
                table: "ReceiptRecord");

            migrationBuilder.DropIndex(
                name: "IX_appointment_ReceiptNavigationId",
                table: "appointment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReceiptRecord",
                table: "ReceiptRecord");

            migrationBuilder.DropIndex(
                name: "IX_ReceiptRecord_ReceiptNavigationId",
                table: "ReceiptRecord");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReceiptModel",
                table: "ReceiptModel");

            migrationBuilder.DropColumn(
                name: "ReceiptNavigationId",
                table: "appointment");

            migrationBuilder.DropColumn(
                name: "ReceiptNavigationId",
                table: "ReceiptRecord");

            migrationBuilder.RenameTable(
                name: "ReceiptRecord",
                newName: "ReceiptRecords");

            migrationBuilder.RenameTable(
                name: "ReceiptModel",
                newName: "Receipts");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReceiptRecords",
                table: "ReceiptRecords",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Receipts",
                table: "Receipts",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_appointment_ReceiptId",
                table: "appointment",
                column: "ReceiptId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptRecords_ReceiptId",
                table: "ReceiptRecords",
                column: "ReceiptId");

            migrationBuilder.AddForeignKey(
                name: "FK_appointment_Receipts_ReceiptId",
                table: "appointment",
                column: "ReceiptId",
                principalTable: "Receipts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiptRecords_Receipts_ReceiptId",
                table: "ReceiptRecords",
                column: "ReceiptId",
                principalTable: "Receipts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_appointment_Receipts_ReceiptId",
                table: "appointment");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptRecords_Receipts_ReceiptId",
                table: "ReceiptRecords");

            migrationBuilder.DropIndex(
                name: "IX_appointment_ReceiptId",
                table: "appointment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Receipts",
                table: "Receipts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReceiptRecords",
                table: "ReceiptRecords");

            migrationBuilder.DropIndex(
                name: "IX_ReceiptRecords_ReceiptId",
                table: "ReceiptRecords");

            migrationBuilder.RenameTable(
                name: "Receipts",
                newName: "ReceiptModel");

            migrationBuilder.RenameTable(
                name: "ReceiptRecords",
                newName: "ReceiptRecord");

            migrationBuilder.AddColumn<int>(
                name: "ReceiptNavigationId",
                table: "appointment",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReceiptNavigationId",
                table: "ReceiptRecord",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReceiptModel",
                table: "ReceiptModel",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReceiptRecord",
                table: "ReceiptRecord",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_appointment_ReceiptNavigationId",
                table: "appointment",
                column: "ReceiptNavigationId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptRecord_ReceiptNavigationId",
                table: "ReceiptRecord",
                column: "ReceiptNavigationId");

            migrationBuilder.AddForeignKey(
                name: "FK_appointment_ReceiptModel_ReceiptNavigationId",
                table: "appointment",
                column: "ReceiptNavigationId",
                principalTable: "ReceiptModel",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiptRecord_ReceiptModel_ReceiptNavigationId",
                table: "ReceiptRecord",
                column: "ReceiptNavigationId",
                principalTable: "ReceiptModel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
