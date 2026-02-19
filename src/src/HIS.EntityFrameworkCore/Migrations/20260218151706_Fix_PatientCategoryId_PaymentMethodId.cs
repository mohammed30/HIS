using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HIS.Migrations
{
    /// <inheritdoc />
    public partial class Fix_PatientCategoryId_PaymentMethodId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppPatients_AppPatientCategories_PatientCategoryId",
                table: "AppPatients");

            migrationBuilder.RenameColumn(
                name: "PatientCategoryId",
                table: "AppPatients",
                newName: "PaymentMethodId");

            migrationBuilder.RenameIndex(
                name: "IX_AppPatients_PatientCategoryId",
                table: "AppPatients",
                newName: "IX_AppPatients_PaymentMethodId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppPatients_AppPaymentMethods_PaymentMethodId",
                table: "AppPatients",
                column: "PaymentMethodId",
                principalTable: "AppPaymentMethods",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppPatients_AppPaymentMethods_PaymentMethodId",
                table: "AppPatients");

            migrationBuilder.RenameColumn(
                name: "PaymentMethodId",
                table: "AppPatients",
                newName: "PatientCategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_AppPatients_PaymentMethodId",
                table: "AppPatients",
                newName: "IX_AppPatients_PatientCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppPatients_AppPatientCategories_PatientCategoryId",
                table: "AppPatients",
                column: "PatientCategoryId",
                principalTable: "AppPatientCategories",
                principalColumn: "Id");
        }
    }
}
