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
            // Safely drop old foreign key
            migrationBuilder.Sql("IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_AppPatients_AppPatientCategories_PatientCategoryId') ALTER TABLE [AppPatients] DROP CONSTRAINT [FK_AppPatients_AppPatientCategories_PatientCategoryId];");

            // Safely rename column
            migrationBuilder.Sql("IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AppPatients') AND name = 'PatientCategoryId') AND NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AppPatients') AND name = 'PaymentMethodId') EXEC sp_rename 'AppPatients.PatientCategoryId', 'PaymentMethodId', 'COLUMN';");

            // Safely rename index
            migrationBuilder.Sql("IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AppPatients_PatientCategoryId' AND object_id = OBJECT_ID('AppPatients')) AND NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AppPatients_PaymentMethodId' AND object_id = OBJECT_ID('AppPatients')) EXEC sp_rename 'AppPatients.IX_AppPatients_PatientCategoryId', 'IX_AppPatients_PaymentMethodId', 'INDEX';");

            // Safely add new foreign key
            migrationBuilder.Sql("IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_AppPatients_AppPaymentMethods_PaymentMethodId') ALTER TABLE [AppPatients] ADD CONSTRAINT [FK_AppPatients_AppPaymentMethods_PaymentMethodId] FOREIGN KEY ([PaymentMethodId]) REFERENCES [AppPaymentMethods] ([Id]);");
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
