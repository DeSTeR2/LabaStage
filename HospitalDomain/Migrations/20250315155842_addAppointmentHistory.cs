using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalDomain.Migrations
{
    /// <inheritdoc />
    public partial class addAppointmentHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppointmentChanges",
                columns: table => new
                {
                    AppointmentId = table.Column<int>(type: "int", nullable: false),
                    ChangeTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChangeInfo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChangedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppointmentChanges", x => new { x.AppointmentId, x.ChangeTime });
                    table.ForeignKey(
                        name: "FK_AppointmentChanges_appointment_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "appointment",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppointmentChanges");
        }
    }
}
