using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HIS.Migrations
{
    /// <inheritdoc />
    public partial class Added_MedicalOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppLabAppointments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AppointmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PreferredTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    PreparationInstructions = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    IsFasting = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_AppLabAppointments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppMedicalOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DoctorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ServiceItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ClinicalNotes = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    Details = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
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
                    table.PrimaryKey("PK_AppMedicalOrders", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppLabAppointments_AppointmentDate",
                table: "AppLabAppointments",
                column: "AppointmentDate");

            migrationBuilder.CreateIndex(
                name: "IX_AppLabAppointments_PatientId",
                table: "AppLabAppointments",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_AppLabAppointments_ServiceItemId",
                table: "AppLabAppointments",
                column: "ServiceItemId");

            migrationBuilder.CreateIndex(
                name: "IX_AppLabAppointments_Status",
                table: "AppLabAppointments",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_AppMedicalOrders_PatientId",
                table: "AppMedicalOrders",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_AppMedicalOrders_ServiceItemId",
                table: "AppMedicalOrders",
                column: "ServiceItemId");

            migrationBuilder.CreateIndex(
                name: "IX_AppMedicalOrders_Status",
                table: "AppMedicalOrders",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppLabAppointments");

            migrationBuilder.DropTable(
                name: "AppMedicalOrders");
        }
    }
}
