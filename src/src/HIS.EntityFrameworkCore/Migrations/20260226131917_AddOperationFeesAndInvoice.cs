using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HIS.Migrations
{
    /// <inheritdoc />
    public partial class AddOperationFeesAndInvoice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AnesthesiologistFeeAmount",
                table: "AppSurgicalOperations",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AnesthesiologistFeePercentage",
                table: "AppSurgicalOperations",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "AnesthesiologistId",
                table: "AppSurgicalOperations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "HospitalShareAmount",
                table: "AppSurgicalOperations",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SurgeonFeeAmount",
                table: "AppSurgicalOperations",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SurgeonFeePercentage",
                table: "AppSurgicalOperations",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnesthesiologistFeeAmount",
                table: "AppSurgicalOperations");

            migrationBuilder.DropColumn(
                name: "AnesthesiologistFeePercentage",
                table: "AppSurgicalOperations");

            migrationBuilder.DropColumn(
                name: "AnesthesiologistId",
                table: "AppSurgicalOperations");

            migrationBuilder.DropColumn(
                name: "HospitalShareAmount",
                table: "AppSurgicalOperations");

            migrationBuilder.DropColumn(
                name: "SurgeonFeeAmount",
                table: "AppSurgicalOperations");

            migrationBuilder.DropColumn(
                name: "SurgeonFeePercentage",
                table: "AppSurgicalOperations");
        }
    }
}
