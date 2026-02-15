using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HIS.Migrations
{
    /// <inheritdoc />
    public partial class AddRoomBedsAndReservations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Amenities",
                table: "AppRooms",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AppBeds",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RoomId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BedNumber = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_AppBeds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppBeds_AppRooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "AppRooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppReservations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoomId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BedId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_AppReservations", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppBeds_RoomId",
                table: "AppBeds",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_AppBeds_RoomId_BedNumber",
                table: "AppBeds",
                columns: new[] { "RoomId", "BedNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppBeds_Status",
                table: "AppBeds",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_AppReservations_BedId",
                table: "AppReservations",
                column: "BedId");

            migrationBuilder.CreateIndex(
                name: "IX_AppReservations_PatientId",
                table: "AppReservations",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_AppReservations_RoomId",
                table: "AppReservations",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_AppReservations_StartDate",
                table: "AppReservations",
                column: "StartDate");

            migrationBuilder.CreateIndex(
                name: "IX_AppReservations_Status",
                table: "AppReservations",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppBeds");

            migrationBuilder.DropTable(
                name: "AppReservations");

            migrationBuilder.DropColumn(
                name: "Amenities",
                table: "AppRooms");
        }
    }
}
