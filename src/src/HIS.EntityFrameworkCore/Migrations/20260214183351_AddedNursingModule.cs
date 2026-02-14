using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HIS.Migrations
{
    /// <inheritdoc />
    public partial class AddedNursingModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppPurchaseOrderLines_AppServiceItems_ProductId",
                table: "AppPurchaseOrderLines");

            migrationBuilder.DropIndex(
                name: "IX_AppSurgicalOperations_PatientId",
                table: "AppSurgicalOperations");

            migrationBuilder.CreateTable(
                name: "AppCarePlans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Diagnosis = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    Goal = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    Interventions = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    Evaluation = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DateCreate = table.Column<DateTime>(type: "datetime2", nullable: false),
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
                    table.PrimaryKey("PK_AppCarePlans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppMedicationAdministrations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MedicalOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AdministrationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Dosage = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    DrugName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
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
                    table.PrimaryKey("PK_AppMedicationAdministrations", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppCarePlans_PatientId",
                table: "AppCarePlans",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_AppCarePlans_Status",
                table: "AppCarePlans",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_AppMedicationAdministrations_AdministrationTime",
                table: "AppMedicationAdministrations",
                column: "AdministrationTime");

            migrationBuilder.CreateIndex(
                name: "IX_AppMedicationAdministrations_MedicalOrderId",
                table: "AppMedicationAdministrations",
                column: "MedicalOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_AppMedicationAdministrations_PatientId",
                table: "AppMedicationAdministrations",
                column: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppPurchaseOrderLines_AppServiceItems_ProductId",
                table: "AppPurchaseOrderLines",
                column: "ProductId",
                principalTable: "AppServiceItems",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppPurchaseOrderLines_AppServiceItems_ProductId",
                table: "AppPurchaseOrderLines");

            migrationBuilder.DropTable(
                name: "AppCarePlans");

            migrationBuilder.DropTable(
                name: "AppMedicationAdministrations");

            migrationBuilder.CreateIndex(
                name: "IX_AppSurgicalOperations_PatientId",
                table: "AppSurgicalOperations",
                column: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppPurchaseOrderLines_AppServiceItems_ProductId",
                table: "AppPurchaseOrderLines",
                column: "ProductId",
                principalTable: "AppServiceItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
