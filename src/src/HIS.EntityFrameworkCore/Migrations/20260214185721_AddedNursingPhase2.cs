using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HIS.Migrations
{
    /// <inheritdoc />
    public partial class AddedNursingPhase2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppFallRiskAssessments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TotalScore = table.Column<int>(type: "int", nullable: false),
                    RiskLevel = table.Column<int>(type: "int", nullable: false),
                    HistoryOfFalls = table.Column<bool>(type: "bit", nullable: false),
                    SecondaryDiagnosis = table.Column<bool>(type: "bit", nullable: false),
                    AmbulatoryAid = table.Column<bool>(type: "bit", nullable: false),
                    IVTherapy = table.Column<bool>(type: "bit", nullable: false),
                    GaitProblem = table.Column<bool>(type: "bit", nullable: false),
                    MentalStatusIssue = table.Column<bool>(type: "bit", nullable: false),
                    AssessmentTime = table.Column<DateTime>(type: "datetime2", nullable: false),
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
                    table.PrimaryKey("PK_AppFallRiskAssessments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppFluidBalances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Metric = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    EntryTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
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
                    table.PrimaryKey("PK_AppFluidBalances", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppPainAssessments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PainScore = table.Column<int>(type: "int", nullable: false),
                    Location = table.Column<int>(type: "int", nullable: false),
                    Characteristics = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    Intervention = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    AssessmentTime = table.Column<DateTime>(type: "datetime2", nullable: false),
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
                    table.PrimaryKey("PK_AppPainAssessments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppPatientRounds",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    NurseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
                    table.PrimaryKey("PK_AppPatientRounds", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppShiftHandovers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Shift = table.Column<int>(type: "int", nullable: false),
                    HandoverTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", maxLength: 4096, nullable: false),
                    OutgoingNurseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IncomingNurseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    table.PrimaryKey("PK_AppShiftHandovers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppWoundCares",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Stage = table.Column<int>(type: "int", nullable: false),
                    Length = table.Column<double>(type: "float", nullable: false),
                    Width = table.Column<double>(type: "float", nullable: false),
                    Depth = table.Column<double>(type: "float", nullable: false),
                    Exudate = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Treatment = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    AssessmentTime = table.Column<DateTime>(type: "datetime2", nullable: false),
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
                    table.PrimaryKey("PK_AppWoundCares", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppFallRiskAssessments_PatientId",
                table: "AppFallRiskAssessments",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_AppFluidBalances_EntryTime",
                table: "AppFluidBalances",
                column: "EntryTime");

            migrationBuilder.CreateIndex(
                name: "IX_AppFluidBalances_PatientId",
                table: "AppFluidBalances",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_AppPainAssessments_PatientId",
                table: "AppPainAssessments",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_AppPatientRounds_PatientId",
                table: "AppPatientRounds",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_AppShiftHandovers_HandoverTime",
                table: "AppShiftHandovers",
                column: "HandoverTime");

            migrationBuilder.CreateIndex(
                name: "IX_AppWoundCares_PatientId",
                table: "AppWoundCares",
                column: "PatientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppFallRiskAssessments");

            migrationBuilder.DropTable(
                name: "AppFluidBalances");

            migrationBuilder.DropTable(
                name: "AppPainAssessments");

            migrationBuilder.DropTable(
                name: "AppPatientRounds");

            migrationBuilder.DropTable(
                name: "AppShiftHandovers");

            migrationBuilder.DropTable(
                name: "AppWoundCares");
        }
    }
}
