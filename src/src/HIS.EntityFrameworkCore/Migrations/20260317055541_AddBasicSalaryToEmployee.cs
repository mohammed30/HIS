using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HIS.Migrations
{
    /// <inheritdoc />
    public partial class AddBasicSalaryToEmployee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "DailyAttendances",
                type: "nvarchar(1024)",
                maxLength: 1024,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "BasicSalary",
                table: "AppEmployees",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DailyAttendances_Date",
                table: "DailyAttendances",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_DailyAttendances_EmployeeId",
                table: "DailyAttendances",
                column: "EmployeeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DailyAttendances_Date",
                table: "DailyAttendances");

            migrationBuilder.DropIndex(
                name: "IX_DailyAttendances_EmployeeId",
                table: "DailyAttendances");

            migrationBuilder.DropColumn(
                name: "BasicSalary",
                table: "AppEmployees");

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "DailyAttendances",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1024)",
                oldMaxLength: 1024,
                oldNullable: true);
        }
    }
}
