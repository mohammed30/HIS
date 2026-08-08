using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HIS.Migrations
{
    /// <inheritdoc />
    public partial class AddInsuranceServicePercentages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "InsurancePercentage",
                table: "AppInvoiceItems",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ConsultationCoveragePercentage",
                table: "AppInsurancePlans",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "InpatientCoveragePercentage",
                table: "AppInsurancePlans",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "LabCoveragePercentage",
                table: "AppInsurancePlans",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MedicalServiceCoveragePercentage",
                table: "AppInsurancePlans",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MedicationsCoveragePercentage",
                table: "AppInsurancePlans",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "OperationsCoveragePercentage",
                table: "AppInsurancePlans",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "RadiologyCoveragePercentage",
                table: "AppInsurancePlans",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InsurancePercentage",
                table: "AppInvoiceItems");

            migrationBuilder.DropColumn(
                name: "ConsultationCoveragePercentage",
                table: "AppInsurancePlans");

            migrationBuilder.DropColumn(
                name: "InpatientCoveragePercentage",
                table: "AppInsurancePlans");

            migrationBuilder.DropColumn(
                name: "LabCoveragePercentage",
                table: "AppInsurancePlans");

            migrationBuilder.DropColumn(
                name: "MedicalServiceCoveragePercentage",
                table: "AppInsurancePlans");

            migrationBuilder.DropColumn(
                name: "MedicationsCoveragePercentage",
                table: "AppInsurancePlans");

            migrationBuilder.DropColumn(
                name: "OperationsCoveragePercentage",
                table: "AppInsurancePlans");

            migrationBuilder.DropColumn(
                name: "RadiologyCoveragePercentage",
                table: "AppInsurancePlans");
        }
    }
}
