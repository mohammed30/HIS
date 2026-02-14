using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HIS.Migrations
{
    /// <inheritdoc />
    public partial class AddedProductToPurchaseOrderLine : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_AppPurchaseOrderLines_AppServiceItems_ProductId",
                table: "AppPurchaseOrderLines",
                column: "ProductId",
                principalTable: "AppServiceItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppPurchaseOrderLines_AppServiceItems_ProductId",
                table: "AppPurchaseOrderLines");
        }
    }
}
