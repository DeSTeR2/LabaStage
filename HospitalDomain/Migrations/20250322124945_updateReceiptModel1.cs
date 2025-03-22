using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalDomain.Migrations
{
    /// <inheritdoc />
    public partial class updateReceiptModel1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AppointmentId",
                table: "Receipts",
                type: "int",
                nullable: false,
                defaultValue: 0);

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
    }
}
