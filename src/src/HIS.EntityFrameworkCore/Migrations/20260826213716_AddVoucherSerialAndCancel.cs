using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HIS.Migrations
{
    /// <inheritdoc />
    public partial class AddVoucherSerialAndCancel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "AppReceiptVouchers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CancellationTime",
                table: "AppReceiptVouchers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CancelledByUserId",
                table: "AppReceiptVouchers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancelledByUserName",
                table: "AppReceiptVouchers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCancelled",
                table: "AppReceiptVouchers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "SerialNumber",
                table: "AppReceiptVouchers",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "AppPaymentVouchers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CancellationTime",
                table: "AppPaymentVouchers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CancelledByUserId",
                table: "AppPaymentVouchers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancelledByUserName",
                table: "AppPaymentVouchers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCancelled",
                table: "AppPaymentVouchers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "SerialNumber",
                table: "AppPaymentVouchers",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "AppReceiptVouchers");

            migrationBuilder.DropColumn(
                name: "CancellationTime",
                table: "AppReceiptVouchers");

            migrationBuilder.DropColumn(
                name: "CancelledByUserId",
                table: "AppReceiptVouchers");

            migrationBuilder.DropColumn(
                name: "CancelledByUserName",
                table: "AppReceiptVouchers");

            migrationBuilder.DropColumn(
                name: "IsCancelled",
                table: "AppReceiptVouchers");

            migrationBuilder.DropColumn(
                name: "SerialNumber",
                table: "AppReceiptVouchers");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "AppPaymentVouchers");

            migrationBuilder.DropColumn(
                name: "CancellationTime",
                table: "AppPaymentVouchers");

            migrationBuilder.DropColumn(
                name: "CancelledByUserId",
                table: "AppPaymentVouchers");

            migrationBuilder.DropColumn(
                name: "CancelledByUserName",
                table: "AppPaymentVouchers");

            migrationBuilder.DropColumn(
                name: "IsCancelled",
                table: "AppPaymentVouchers");

            migrationBuilder.DropColumn(
                name: "SerialNumber",
                table: "AppPaymentVouchers");
        }
    }
}
