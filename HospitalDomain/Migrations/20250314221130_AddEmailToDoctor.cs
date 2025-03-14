using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalDomain.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailToDoctor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "department",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "char(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                    Location = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_department_1", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "patient",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "char(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                    date_of_birth = table.Column<DateTime>(type: "datetime2(4)", precision: 4, nullable: true),
                    contacts = table.Column<string>(type: "varchar(45)", unicode: false, maxLength: 45, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__patient__3213E83FE0555260", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "room",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false),
                    type = table.Column<string>(type: "varchar(45)", unicode: false, maxLength: 45, nullable: false),
                    capacity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__room__3213E83F9EB6B491", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "doctor",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "char(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                    Speciality = table.Column<string>(type: "char(45)", unicode: false, fixedLength: true, maxLength: 45, nullable: false),
                    Contact = table.Column<string>(type: "varchar(45)", unicode: false, maxLength: 45, nullable: false),
                    Email = table.Column<string>(type: "char(50)", unicode: false, fixedLength: true, maxLength: 50, nullable: false),
                    Department = table.Column<int>(type: "int", nullable: true),
                    ProfilePictureUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__doctor__3213E83F4AF846E6", x => x.id);
                    table.ForeignKey(
                        name: "FK_doctor_department",
                        column: x => x.Department,
                        principalTable: "department",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "appointment",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false),
                    date = table.Column<DateOnly>(type: "date", nullable: false),
                    time = table.Column<TimeOnly>(type: "time", nullable: false),
                    reason = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    doctor = table.Column<int>(type: "int", nullable: false),
                    patient = table.Column<int>(type: "int", nullable: false),
                    room = table.Column<int>(type: "int", nullable: true),
                    AppointmentState = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__appointm__3213E83FBAFA03D0", x => x.id);
                    table.ForeignKey(
                        name: "FK_appointment_appointment",
                        column: x => x.room,
                        principalTable: "room",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_appointment_doctor",
                        column: x => x.doctor,
                        principalTable: "doctor",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_appointment_patient",
                        column: x => x.patient,
                        principalTable: "patient",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_appointment_doctor",
                table: "appointment",
                column: "doctor");

            migrationBuilder.CreateIndex(
                name: "IX_appointment_patient",
                table: "appointment",
                column: "patient");

            migrationBuilder.CreateIndex(
                name: "IX_appointment_room",
                table: "appointment",
                column: "room");

            migrationBuilder.CreateIndex(
                name: "UQ_department_id",
                table: "department",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_doctor_Department",
                table: "doctor",
                column: "Department");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "appointment");

            migrationBuilder.DropTable(
                name: "room");

            migrationBuilder.DropTable(
                name: "doctor");

            migrationBuilder.DropTable(
                name: "patient");

            migrationBuilder.DropTable(
                name: "department");
        }
    }
}
