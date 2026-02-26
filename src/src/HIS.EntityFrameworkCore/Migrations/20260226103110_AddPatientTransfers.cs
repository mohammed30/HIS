using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HIS.Migrations
{
    /// <inheritdoc />
    public partial class AddPatientTransfers_RenameTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PatientTransfers",
                table: "PatientTransfers");

            migrationBuilder.RenameTable(
                name: "PatientTransfers",
                newName: "AppPatientTransfers");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppPatientTransfers",
                table: "AppPatientTransfers",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AppPatientTransfers_AdmissionId",
                table: "AppPatientTransfers",
                column: "AdmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_AppPatientTransfers_FromRoomId",
                table: "AppPatientTransfers",
                column: "FromRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_AppPatientTransfers_ToRoomId",
                table: "AppPatientTransfers",
                column: "ToRoomId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AppPatientTransfers",
                table: "AppPatientTransfers");

            migrationBuilder.DropIndex(
                name: "IX_AppPatientTransfers_AdmissionId",
                table: "AppPatientTransfers");

            migrationBuilder.DropIndex(
                name: "IX_AppPatientTransfers_FromRoomId",
                table: "AppPatientTransfers");

            migrationBuilder.DropIndex(
                name: "IX_AppPatientTransfers_ToRoomId",
                table: "AppPatientTransfers");

            migrationBuilder.RenameTable(
                name: "AppPatientTransfers",
                newName: "PatientTransfers");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PatientTransfers",
                table: "PatientTransfers",
                column: "Id");
        }
    }
}
