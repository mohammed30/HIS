using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HIS.Migrations
{
    /// <inheritdoc />
    public partial class Add_Patient_Fields_And_Master_Data : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "AppPatients");

            migrationBuilder.RenameColumn(
                name: "Nationality",
                table: "AppPatients",
                newName: "TaxFile");

            migrationBuilder.AddColumn<string>(
                name: "CardNumber",
                table: "AppPatients",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ContractId",
                table: "AppPatients",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "IdentityIssueDate",
                table: "AppPatients",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdentityIssuePlace",
                table: "AppPatients",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "NationalityId",
                table: "AppPatients",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PassportExpiryDate",
                table: "AppPatients",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PassportIssueDate",
                table: "AppPatients",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PassportIssuePlace",
                table: "AppPatients",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PassportNumber",
                table: "AppPatients",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PatientCategoryId",
                table: "AppPatients",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProfessionId",
                table: "AppPatients",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ReferralSourceId",
                table: "AppPatients",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SponsorId",
                table: "AppPatients",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SponsorName",
                table: "AppPatients",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "VisaExpiryDate",
                table: "AppPatients",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "VisaIssueDate",
                table: "AppPatients",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VisaIssuePlace",
                table: "AppPatients",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VisaNumber",
                table: "AppPatients",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AppContracts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NameAr = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
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
                    table.PrimaryKey("PK_AppContracts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppNationalities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NameAr = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
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
                    table.PrimaryKey("PK_AppNationalities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppPatientCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NameAr = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
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
                    table.PrimaryKey("PK_AppPatientCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppProfessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NameAr = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
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
                    table.PrimaryKey("PK_AppProfessions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppReferralSources",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NameAr = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
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
                    table.PrimaryKey("PK_AppReferralSources", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppPatients_ContractId",
                table: "AppPatients",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_AppPatients_NationalityId",
                table: "AppPatients",
                column: "NationalityId");

            migrationBuilder.CreateIndex(
                name: "IX_AppPatients_PatientCategoryId",
                table: "AppPatients",
                column: "PatientCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_AppPatients_ProfessionId",
                table: "AppPatients",
                column: "ProfessionId");

            migrationBuilder.CreateIndex(
                name: "IX_AppPatients_ReferralSourceId",
                table: "AppPatients",
                column: "ReferralSourceId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppPatients_AppContracts_ContractId",
                table: "AppPatients",
                column: "ContractId",
                principalTable: "AppContracts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppPatients_AppNationalities_NationalityId",
                table: "AppPatients",
                column: "NationalityId",
                principalTable: "AppNationalities",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppPatients_AppPatientCategories_PatientCategoryId",
                table: "AppPatients",
                column: "PatientCategoryId",
                principalTable: "AppPatientCategories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppPatients_AppProfessions_ProfessionId",
                table: "AppPatients",
                column: "ProfessionId",
                principalTable: "AppProfessions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppPatients_AppReferralSources_ReferralSourceId",
                table: "AppPatients",
                column: "ReferralSourceId",
                principalTable: "AppReferralSources",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppPatients_AppContracts_ContractId",
                table: "AppPatients");

            migrationBuilder.DropForeignKey(
                name: "FK_AppPatients_AppNationalities_NationalityId",
                table: "AppPatients");

            migrationBuilder.DropForeignKey(
                name: "FK_AppPatients_AppPatientCategories_PatientCategoryId",
                table: "AppPatients");

            migrationBuilder.DropForeignKey(
                name: "FK_AppPatients_AppProfessions_ProfessionId",
                table: "AppPatients");

            migrationBuilder.DropForeignKey(
                name: "FK_AppPatients_AppReferralSources_ReferralSourceId",
                table: "AppPatients");

            migrationBuilder.DropTable(
                name: "AppContracts");

            migrationBuilder.DropTable(
                name: "AppNationalities");

            migrationBuilder.DropTable(
                name: "AppPatientCategories");

            migrationBuilder.DropTable(
                name: "AppProfessions");

            migrationBuilder.DropTable(
                name: "AppReferralSources");

            migrationBuilder.DropIndex(
                name: "IX_AppPatients_ContractId",
                table: "AppPatients");

            migrationBuilder.DropIndex(
                name: "IX_AppPatients_NationalityId",
                table: "AppPatients");

            migrationBuilder.DropIndex(
                name: "IX_AppPatients_PatientCategoryId",
                table: "AppPatients");

            migrationBuilder.DropIndex(
                name: "IX_AppPatients_ProfessionId",
                table: "AppPatients");

            migrationBuilder.DropIndex(
                name: "IX_AppPatients_ReferralSourceId",
                table: "AppPatients");

            migrationBuilder.DropColumn(
                name: "CardNumber",
                table: "AppPatients");

            migrationBuilder.DropColumn(
                name: "ContractId",
                table: "AppPatients");

            migrationBuilder.DropColumn(
                name: "IdentityIssueDate",
                table: "AppPatients");

            migrationBuilder.DropColumn(
                name: "IdentityIssuePlace",
                table: "AppPatients");

            migrationBuilder.DropColumn(
                name: "NationalityId",
                table: "AppPatients");

            migrationBuilder.DropColumn(
                name: "PassportExpiryDate",
                table: "AppPatients");

            migrationBuilder.DropColumn(
                name: "PassportIssueDate",
                table: "AppPatients");

            migrationBuilder.DropColumn(
                name: "PassportIssuePlace",
                table: "AppPatients");

            migrationBuilder.DropColumn(
                name: "PassportNumber",
                table: "AppPatients");

            migrationBuilder.DropColumn(
                name: "PatientCategoryId",
                table: "AppPatients");

            migrationBuilder.DropColumn(
                name: "ProfessionId",
                table: "AppPatients");

            migrationBuilder.DropColumn(
                name: "ReferralSourceId",
                table: "AppPatients");

            migrationBuilder.DropColumn(
                name: "SponsorId",
                table: "AppPatients");

            migrationBuilder.DropColumn(
                name: "SponsorName",
                table: "AppPatients");

            migrationBuilder.DropColumn(
                name: "VisaExpiryDate",
                table: "AppPatients");

            migrationBuilder.DropColumn(
                name: "VisaIssueDate",
                table: "AppPatients");

            migrationBuilder.DropColumn(
                name: "VisaIssuePlace",
                table: "AppPatients");

            migrationBuilder.DropColumn(
                name: "VisaNumber",
                table: "AppPatients");

            migrationBuilder.RenameColumn(
                name: "TaxFile",
                table: "AppPatients",
                newName: "Nationality");

            migrationBuilder.AddColumn<int>(
                name: "Category",
                table: "AppPatients",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
