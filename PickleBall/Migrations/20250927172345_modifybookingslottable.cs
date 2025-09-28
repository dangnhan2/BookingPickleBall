using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PickleBall.Migrations
{
    /// <inheritdoc />
    public partial class modifybookingslottable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingTimeSlots_CourtTimeSlots_CourtTimeSlotCourtID_CourtT~",
                table: "BookingTimeSlots");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CourtTimeSlots",
                table: "CourtTimeSlots");

            migrationBuilder.DropIndex(
                name: "IX_BookingTimeSlots_CourtTimeSlotCourtID_CourtTimeSlotTimeSlot~",
                table: "BookingTimeSlots");

            migrationBuilder.DropColumn(
                name: "CourtTimeSlotCourtID",
                table: "BookingTimeSlots");

            migrationBuilder.DropColumn(
                name: "CourtTimeSlotTimeSlotID",
                table: "BookingTimeSlots");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourtTimeSlots",
                table: "CourtTimeSlots",
                column: "ID");

            migrationBuilder.CreateIndex(
                name: "IX_CourtTimeSlots_CourtID",
                table: "CourtTimeSlots",
                column: "CourtID");

            migrationBuilder.CreateIndex(
                name: "IX_BookingTimeSlots_CourtTimeSlotId",
                table: "BookingTimeSlots",
                column: "CourtTimeSlotId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingTimeSlots_CourtTimeSlots_CourtTimeSlotId",
                table: "BookingTimeSlots",
                column: "CourtTimeSlotId",
                principalTable: "CourtTimeSlots",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingTimeSlots_CourtTimeSlots_CourtTimeSlotId",
                table: "BookingTimeSlots");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CourtTimeSlots",
                table: "CourtTimeSlots");

            migrationBuilder.DropIndex(
                name: "IX_CourtTimeSlots_CourtID",
                table: "CourtTimeSlots");

            migrationBuilder.DropIndex(
                name: "IX_BookingTimeSlots_CourtTimeSlotId",
                table: "BookingTimeSlots");

            migrationBuilder.AddColumn<Guid>(
                name: "CourtTimeSlotCourtID",
                table: "BookingTimeSlots",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CourtTimeSlotTimeSlotID",
                table: "BookingTimeSlots",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourtTimeSlots",
                table: "CourtTimeSlots",
                columns: new[] { "CourtID", "TimeSlotID" });

            migrationBuilder.CreateIndex(
                name: "IX_BookingTimeSlots_CourtTimeSlotCourtID_CourtTimeSlotTimeSlot~",
                table: "BookingTimeSlots",
                columns: new[] { "CourtTimeSlotCourtID", "CourtTimeSlotTimeSlotID" });

            migrationBuilder.AddForeignKey(
                name: "FK_BookingTimeSlots_CourtTimeSlots_CourtTimeSlotCourtID_CourtT~",
                table: "BookingTimeSlots",
                columns: new[] { "CourtTimeSlotCourtID", "CourtTimeSlotTimeSlotID" },
                principalTable: "CourtTimeSlots",
                principalColumns: new[] { "CourtID", "TimeSlotID" },
                onDelete: ReferentialAction.Cascade);
        }
    }
}
