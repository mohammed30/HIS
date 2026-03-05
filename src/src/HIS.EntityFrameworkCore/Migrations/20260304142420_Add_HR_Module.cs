using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HIS.Migrations
{
    /// <inheritdoc />
    public partial class Add_HR_Module : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppAttendanceRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PermitType = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Hours = table.Column<int>(type: "int", nullable: true),
                    Minutes = table.Column<int>(type: "int", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
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
                    table.PrimaryKey("PK_AppAttendanceRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppCompensationItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NameAr = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Nature = table.Column<int>(type: "int", nullable: false),
                    ValueType = table.Column<int>(type: "int", nullable: false),
                    Method = table.Column<int>(type: "int", nullable: false),
                    FormulaExpression = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_AppCompensationItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppEmployeeLeaves",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LeaveTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    Entitled = table.Column<int>(type: "int", nullable: false),
                    Used = table.Column<int>(type: "int", nullable: false),
                    Balance = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
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
                    table.PrimaryKey("PK_AppEmployeeLeaves", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppEmployeeLoans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CompensationItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Installments = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PaidAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
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
                    table.PrimaryKey("PK_AppEmployeeLoans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppEmployees",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EmployeeNumber = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MaritalStatus = table.Column<int>(type: "int", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Qualification = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IdentityType = table.Column<int>(type: "int", nullable: true),
                    IdentityNumber = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    InsuranceNumber = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    BankName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    BankAccountNumber = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SectionName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    JobGradeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    JobTitle = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmploymentClassification = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    SalaryPaymentMethod = table.Column<int>(type: "int", nullable: true),
                    ContractType = table.Column<int>(type: "int", nullable: true),
                    HireDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TerminationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReminderEnabled = table.Column<bool>(type: "bit", nullable: false),
                    PhotoUrl = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    IsSuspended = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_AppEmployees", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppJobGrades",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    BaseSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_AppJobGrades", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppLeaveTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NameAr = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    EmployeeClass = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    AffectsSalary = table.Column<bool>(type: "bit", nullable: false),
                    IsBalance = table.Column<bool>(type: "bit", nullable: false),
                    IsPublicHoliday = table.Column<bool>(type: "bit", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_AppLeaveTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppPayrollRuns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PeriodStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PeriodEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    JobGradeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    JournalEntryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TotalEarnings = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalDeductions = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NetSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
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
                    table.PrimaryKey("PK_AppPayrollRuns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppPenalties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PenaltyType = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SuspensionDays = table.Column<int>(type: "int", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
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
                    table.PrimaryKey("PK_AppPenalties", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppSalarySetups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompensationItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsRecurring = table.Column<bool>(type: "bit", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    table.PrimaryKey("PK_AppSalarySetups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppPayrollLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PayrollRunId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompensationItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Nature = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_AppPayrollLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppPayrollLines_AppPayrollRuns_PayrollRunId",
                        column: x => x.PayrollRunId,
                        principalTable: "AppPayrollRuns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppAttendanceRecords_Date",
                table: "AppAttendanceRecords",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_AppAttendanceRecords_EmployeeId",
                table: "AppAttendanceRecords",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_AppCompensationItems_AccountId",
                table: "AppCompensationItems",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AppEmployeeLeaves_EmployeeId",
                table: "AppEmployeeLeaves",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_AppEmployeeLeaves_LeaveTypeId",
                table: "AppEmployeeLeaves",
                column: "LeaveTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AppEmployeeLoans_EmployeeId",
                table: "AppEmployeeLoans",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_AppEmployees_DepartmentId",
                table: "AppEmployees",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AppEmployees_EmployeeNumber",
                table: "AppEmployees",
                column: "EmployeeNumber");

            migrationBuilder.CreateIndex(
                name: "IX_AppEmployees_JobGradeId",
                table: "AppEmployees",
                column: "JobGradeId");

            migrationBuilder.CreateIndex(
                name: "IX_AppJobGrades_Code",
                table: "AppJobGrades",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_AppPayrollLines_EmployeeId",
                table: "AppPayrollLines",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_AppPayrollLines_PayrollRunId",
                table: "AppPayrollLines",
                column: "PayrollRunId");

            migrationBuilder.CreateIndex(
                name: "IX_AppPayrollRuns_DepartmentId",
                table: "AppPayrollRuns",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AppPayrollRuns_JournalEntryId",
                table: "AppPayrollRuns",
                column: "JournalEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_AppPenalties_Date",
                table: "AppPenalties",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_AppPenalties_EmployeeId",
                table: "AppPenalties",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_AppSalarySetups_CompensationItemId",
                table: "AppSalarySetups",
                column: "CompensationItemId");

            migrationBuilder.CreateIndex(
                name: "IX_AppSalarySetups_EmployeeId",
                table: "AppSalarySetups",
                column: "EmployeeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppAttendanceRecords");

            migrationBuilder.DropTable(
                name: "AppCompensationItems");

            migrationBuilder.DropTable(
                name: "AppEmployeeLeaves");

            migrationBuilder.DropTable(
                name: "AppEmployeeLoans");

            migrationBuilder.DropTable(
                name: "AppEmployees");

            migrationBuilder.DropTable(
                name: "AppJobGrades");

            migrationBuilder.DropTable(
                name: "AppLeaveTypes");

            migrationBuilder.DropTable(
                name: "AppPayrollLines");

            migrationBuilder.DropTable(
                name: "AppPenalties");

            migrationBuilder.DropTable(
                name: "AppSalarySetups");

            migrationBuilder.DropTable(
                name: "AppPayrollRuns");
        }
    }
}
