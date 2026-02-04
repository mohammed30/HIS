using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HIS.Migrations
{
    /// <inheritdoc />
    public partial class AddLabFieldsToServiceItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TestId",
                table: "AppLabRequests",
                newName: "ServiceItemId");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "AppServiceItems",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ReferenceRange",
                table: "AppServiceItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "AppServiceItems",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "AppServiceItems");

            migrationBuilder.DropColumn(
                name: "ReferenceRange",
                table: "AppServiceItems");

            migrationBuilder.DropColumn(
                name: "Unit",
                table: "AppServiceItems");

            migrationBuilder.RenameColumn(
                name: "ServiceItemId",
                table: "AppLabRequests",
                newName: "TestId");
        }
    }
}
