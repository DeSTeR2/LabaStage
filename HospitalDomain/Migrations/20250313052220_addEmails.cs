using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalDomain.Migrations
{
    /// <inheritdoc />
    public partial class addEmails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_appointment_patient_RoomNavigationId",
                table: "appointment");

            migrationBuilder.DropForeignKey(
                name: "FK_room_appointment_AppointmentId",
                table: "room");


            migrationBuilder.DropPrimaryKey(
                name: "PK__appointment__3213E83FBAFA03D0",
                table: "appointment");

            migrationBuilder.DropIndex(
                name: "IX_appointment_RoomNavigationId",
                table: "appointment");

            migrationBuilder.DropColumn(
                name: "AppointmentId",
                table: "room");

            migrationBuilder.DropColumn(
                name: "RoomNavigationId",
                table: "appointment");



            migrationBuilder.RenameColumn(
                name: "Room",
                table: "appointment",
                newName: "room");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_of_birth",
                table: "patient",
                type: "datetime2(4)",
                precision: 4,
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2(4)",
                oldPrecision: 4);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "patient",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "doctor",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");



            migrationBuilder.AddPrimaryKey(
                name: "PK__appointm__3213E83FBAFA03D0",
                table: "appointment",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_appointment_room",
                table: "appointment",
                column: "room");

            migrationBuilder.AddForeignKey(
                name: "FK_appointment_appointment",
                table: "appointment",
                column: "room",
                principalTable: "room",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_appointment_appointment",
                table: "appointment");



            migrationBuilder.DropPrimaryKey(
                name: "PK__appointm__3213E83FBAFA03D0",
                table: "appointment");

            migrationBuilder.DropIndex(
                name: "IX_appointment_room",
                table: "appointment");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "patient");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "doctor");





            migrationBuilder.RenameColumn(
                name: "room",
                table: "appointment",
                newName: "Room");

            migrationBuilder.AddColumn<int>(
                name: "AppointmentId",
                table: "room",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_of_birth",
                table: "patient",
                type: "datetime2(4)",
                precision: 4,
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2(4)",
                oldPrecision: 4,
                oldNullable: true);

            

            migrationBuilder.AddColumn<int>(
                name: "RoomNavigationId",
                table: "appointment",
                type: "int",
                nullable: false,
                defaultValue: 0);


            migrationBuilder.AddPrimaryKey(
                name: "PK__appointment__3213E83FBAFA03D0",
                table: "appointment",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_appointment_RoomNavigationId",
                table: "appointment",
                column: "RoomNavigationId");

            migrationBuilder.AddForeignKey(
                name: "FK_appointment_patient_RoomNavigationId",
                table: "appointment",
                column: "RoomNavigationId",
                principalTable: "patient",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_room_appointment_AppointmentId",
                table: "room",
                column: "AppointmentId",
                principalTable: "appointment",
                principalColumn: "id");
        }
    }
}
