using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HIS.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailAndIBANToEmployee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "OvertimeHours",
                table: "DailyAttendances",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "AdmissionId",
                table: "AppMedicalOrders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SampleNumber",
                table: "AppLabRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PlanClass",
                table: "AppInsurancePlans",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "AppEmployees",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IBAN",
                table: "AppEmployees",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ClinicId",
                table: "AppDoctors",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PatientInsuranceId",
                table: "AppAdmissions",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OvertimeHours",
                table: "DailyAttendances");

            migrationBuilder.DropColumn(
                name: "AdmissionId",
                table: "AppMedicalOrders");

            migrationBuilder.DropColumn(
                name: "SampleNumber",
                table: "AppLabRequests");

            migrationBuilder.DropColumn(
                name: "PlanClass",
                table: "AppInsurancePlans");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "AppEmployees");

            migrationBuilder.DropColumn(
                name: "IBAN",
                table: "AppEmployees");

            migrationBuilder.DropColumn(
                name: "ClinicId",
                table: "AppDoctors");

            migrationBuilder.DropColumn(
                name: "PatientInsuranceId",
                table: "AppAdmissions");
        }
    }
}
