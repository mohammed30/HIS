using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HIS.Migrations
{
    /// <inheritdoc />
    public partial class AddCostCenters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CostCenterId",
                table: "AppJournalEntryLines",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DepartmentId",
                table: "AppInvoiceItems",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AppCostCenters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppCostCenters", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppDepartments_CostCenterId",
                table: "AppDepartments",
                column: "CostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_AppCostCenters_Code",
                table: "AppCostCenters",
                column: "Code",
                unique: true);

            // Set existing CostCenterId to null to avoid FK constraint conflict
            migrationBuilder.Sql("UPDATE AppDepartments SET CostCenterId = NULL;");

            migrationBuilder.AddForeignKey(
                name: "FK_AppDepartments_AppCostCenters_CostCenterId",
                table: "AppDepartments",
                column: "CostCenterId",
                principalTable: "AppCostCenters",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppDepartments_AppCostCenters_CostCenterId",
                table: "AppDepartments");

            migrationBuilder.DropTable(
                name: "AppCostCenters");

            migrationBuilder.DropIndex(
                name: "IX_AppDepartments_CostCenterId",
                table: "AppDepartments");

            migrationBuilder.DropColumn(
                name: "CostCenterId",
                table: "AppJournalEntryLines");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "AppInvoiceItems");
        }
    }
}
