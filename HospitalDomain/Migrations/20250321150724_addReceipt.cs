using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalDomain.Migrations
{
    /// <inheritdoc />
    public partial class addReceipt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReceiptId",
                table: "appointment",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReceiptNavigationId",
                table: "appointment",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ReceiptModel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceiptModel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReceiptRecord",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amound = table.Column<int>(type: "int", nullable: true),
                    ReceiptId = table.Column<int>(type: "int", nullable: false),
                    ReceiptNavigationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceiptRecord", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReceiptRecord_ReceiptModel_ReceiptNavigationId",
                        column: x => x.ReceiptNavigationId,
                        principalTable: "ReceiptModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_appointment_ReceiptModel_ReceiptNavigationId",
                table: "appointment");

            migrationBuilder.DropTable(
                name: "ReceiptRecord");

            migrationBuilder.DropTable(
                name: "ReceiptModel");

            migrationBuilder.DropIndex(
                name: "IX_appointment_ReceiptNavigationId",
                table: "appointment");

            migrationBuilder.DropColumn(
                name: "ReceiptId",
                table: "appointment");

            migrationBuilder.DropColumn(
                name: "ReceiptNavigationId",
                table: "appointment");
        }
    }
}
