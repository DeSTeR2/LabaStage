using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalDomain.Migrations
{
    /// <inheritdoc />
    public partial class updateReceiptModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amound",
                table: "ReceiptRecords");

            migrationBuilder.AddColumn<int>(
                name: "AppointmentId",
                table: "Receipts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ReceiptRecords",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_AppointmentId",
                table: "Receipts",
                column: "AppointmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Receipts_appointment_AppointmentId",
                table: "Receipts",
                column: "AppointmentId",
                principalTable: "appointment",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Receipts_appointment_AppointmentId",
                table: "Receipts");

            migrationBuilder.DropIndex(
                name: "IX_Receipts_AppointmentId",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "AppointmentId",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "ReceiptRecords");

            migrationBuilder.AddColumn<int>(
                name: "Amound",
                table: "ReceiptRecords",
                type: "int",
                nullable: true);
        }
    }
}
