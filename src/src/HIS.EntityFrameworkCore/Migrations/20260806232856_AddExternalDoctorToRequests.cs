using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HIS.Migrations
{
    /// <inheritdoc />
    public partial class AddExternalDoctorToRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExternalDoctorName",
                table: "AppRadiologyRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsExternalDoctor",
                table: "AppRadiologyRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<Guid>(
                name: "DoctorId",
                table: "AppLabRequests",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<string>(
                name: "ExternalDoctorName",
                table: "AppLabRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsExternalDoctor",
                table: "AppLabRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalDoctorName",
                table: "AppRadiologyRequests");

            migrationBuilder.DropColumn(
                name: "IsExternalDoctor",
                table: "AppRadiologyRequests");

            migrationBuilder.DropColumn(
                name: "ExternalDoctorName",
                table: "AppLabRequests");

            migrationBuilder.DropColumn(
                name: "IsExternalDoctor",
                table: "AppLabRequests");

            migrationBuilder.AlterColumn<Guid>(
                name: "DoctorId",
                table: "AppLabRequests",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }
    }
}
