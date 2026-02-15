using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HIS.Migrations
{
    /// <inheritdoc />
    public partial class Add_Inventory_StockLevels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "MinStockLevel",
                table: "AppInventoryItems",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ReorderLevel",
                table: "AppInventoryItems",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsControlled",
                table: "AppDrugs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LegalCategory",
                table: "AppDrugs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CounselingNotes",
                table: "AppDispensings",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MinStockLevel",
                table: "AppInventoryItems");

            migrationBuilder.DropColumn(
                name: "ReorderLevel",
                table: "AppInventoryItems");

            migrationBuilder.DropColumn(
                name: "IsControlled",
                table: "AppDrugs");

            migrationBuilder.DropColumn(
                name: "LegalCategory",
                table: "AppDrugs");

            migrationBuilder.DropColumn(
                name: "CounselingNotes",
                table: "AppDispensings");
        }
    }
}
