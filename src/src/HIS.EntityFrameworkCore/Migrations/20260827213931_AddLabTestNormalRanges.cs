using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HIS.Migrations
{
    /// <inheritdoc />
    public partial class AddLabTestNormalRanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppLabTestNormalRanges",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LabTestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TargetGender = table.Column<int>(type: "int", nullable: true),
                    MinAgeDays = table.Column<int>(type: "int", nullable: true),
                    MaxAgeDays = table.Column<int>(type: "int", nullable: true),
                    ResultType = table.Column<int>(type: "int", nullable: false),
                    MinValue = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    MaxValue = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    NormalStringValue = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
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
                    table.PrimaryKey("PK_AppLabTestNormalRanges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppLabTestNormalRanges_AppLabTests_LabTestId",
                        column: x => x.LabTestId,
                        principalTable: "AppLabTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppLabTestNormalRanges_LabTestId",
                table: "AppLabTestNormalRanges",
                column: "LabTestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppLabTestNormalRanges");
        }
    }
}
