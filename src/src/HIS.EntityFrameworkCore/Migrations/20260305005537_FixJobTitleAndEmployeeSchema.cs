using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HIS.Migrations
{
    /// <inheritdoc />
    public partial class FixJobTitleAndEmployeeSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "NameEn",
                table: "AppJobTitles",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "AppJobTitles",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(512)",
                oldMaxLength: 512);

            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AppEmployees') AND name = 'JobTitleId')
BEGIN
    ALTER TABLE [AppEmployees] ADD [JobTitleId] uniqueidentifier NULL;
END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JobTitleId",
                table: "AppEmployees");

            migrationBuilder.AlterColumn<string>(
                name: "NameEn",
                table: "AppJobTitles",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "AppJobTitles",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(512)",
                oldMaxLength: 512,
                oldNullable: true);
        }
    }
}
