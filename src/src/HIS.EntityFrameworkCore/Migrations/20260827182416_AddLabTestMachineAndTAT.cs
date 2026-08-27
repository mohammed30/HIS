using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HIS.Migrations
{
    /// <inheritdoc />
    public partial class AddLabTestMachineAndTAT : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Machine",
                table: "AppLabTests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TurnaroundTime",
                table: "AppLabTests",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Machine",
                table: "AppLabTests");

            migrationBuilder.DropColumn(
                name: "TurnaroundTime",
                table: "AppLabTests");
        }
    }
}
