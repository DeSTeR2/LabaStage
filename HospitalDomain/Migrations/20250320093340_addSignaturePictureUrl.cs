using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalDomain.Migrations
{
    /// <inheritdoc />
    public partial class addSignaturePictureUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SignaturePictireUrl",
                table: "doctor",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SignaturePictireUrl",
                table: "doctor");
        }
    }
}
