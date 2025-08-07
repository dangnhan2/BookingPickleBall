using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PickleBall.Migrations
{
    /// <inheritdoc />
    public partial class AddTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TimeSlots_Courts_CourtId",
                table: "TimeSlots");

            migrationBuilder.DropIndex(
                name: "IX_TimeSlots_CourtId",
                table: "TimeSlots");

            migrationBuilder.DropColumn(
                name: "CourtId",
                table: "TimeSlots");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Courts");

            migrationBuilder.CreateTable(
                name: "CourtTimeSlots",
                columns: table => new
                {
                    CourtID = table.Column<Guid>(type: "uuid", nullable: false),
                    TimeSlotID = table.Column<Guid>(type: "uuid", nullable: false),
                    ID = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourtTimeSlots", x => new { x.CourtID, x.TimeSlotID });
                    table.ForeignKey(
                        name: "FK_CourtTimeSlots_Courts_CourtID",
                        column: x => x.CourtID,
                        principalTable: "Courts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourtTimeSlots_TimeSlots_TimeSlotID",
                        column: x => x.TimeSlotID,
                        principalTable: "TimeSlots",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CourtTimeSlots_TimeSlotID",
                table: "CourtTimeSlots",
                column: "TimeSlotID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourtTimeSlots");

            migrationBuilder.AddColumn<Guid>(
                name: "CourtId",
                table: "TimeSlots",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Courts",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_TimeSlots_CourtId",
                table: "TimeSlots",
                column: "CourtId");

            migrationBuilder.AddForeignKey(
                name: "FK_TimeSlots_Courts_CourtId",
                table: "TimeSlots",
                column: "CourtId",
                principalTable: "Courts",
                principalColumn: "ID");
        }
    }
}
