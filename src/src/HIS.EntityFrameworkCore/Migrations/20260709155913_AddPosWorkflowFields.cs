using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HIS.Migrations
{
    /// <inheritdoc />
    public partial class AddPosWorkflowFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InvoiceType",
                table: "AppInvoices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "OriginalInvoiceId",
                table: "AppInvoices",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OriginalInvoiceNumber",
                table: "AppInvoices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaymentMethod",
                table: "AppInvoices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "AppInvoices",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InvoiceType",
                table: "AppInvoices");

            migrationBuilder.DropColumn(
                name: "OriginalInvoiceId",
                table: "AppInvoices");

            migrationBuilder.DropColumn(
                name: "OriginalInvoiceNumber",
                table: "AppInvoices");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "AppInvoices");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "AppInvoices");
        }
    }
}
