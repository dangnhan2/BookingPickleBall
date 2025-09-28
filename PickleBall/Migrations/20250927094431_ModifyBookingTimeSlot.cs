using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PickleBall.Migrations
{
    /// <inheritdoc />
    public partial class ModifyBookingTimeSlot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingTimeSlots_TimeSlots_TimeSlotId",
                table: "BookingTimeSlots");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BookingTimeSlots",
                table: "BookingTimeSlots");

            migrationBuilder.RenameColumn(
                name: "TimeSlotId",
                table: "BookingTimeSlots",
                newName: "TimeSlotID");

            migrationBuilder.RenameIndex(
                name: "IX_BookingTimeSlots_TimeSlotId",
                table: "BookingTimeSlots",
                newName: "IX_BookingTimeSlots_TimeSlotID");

            migrationBuilder.AlterColumn<Guid>(
                name: "TimeSlotID",
                table: "BookingTimeSlots",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "CourtTimeSlotId",
                table: "BookingTimeSlots",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

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
                name: "PK_BookingTimeSlots",
                table: "BookingTimeSlots",
                columns: new[] { "BookingId", "CourtTimeSlotId" });

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

            migrationBuilder.AddForeignKey(
                name: "FK_BookingTimeSlots_TimeSlots_TimeSlotID",
                table: "BookingTimeSlots",
                column: "TimeSlotID",
                principalTable: "TimeSlots",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingTimeSlots_CourtTimeSlots_CourtTimeSlotCourtID_CourtT~",
                table: "BookingTimeSlots");

            migrationBuilder.DropForeignKey(
                name: "FK_BookingTimeSlots_TimeSlots_TimeSlotID",
                table: "BookingTimeSlots");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BookingTimeSlots",
                table: "BookingTimeSlots");

            migrationBuilder.DropIndex(
                name: "IX_BookingTimeSlots_CourtTimeSlotCourtID_CourtTimeSlotTimeSlot~",
                table: "BookingTimeSlots");

            migrationBuilder.DropColumn(
                name: "CourtTimeSlotId",
                table: "BookingTimeSlots");

            migrationBuilder.DropColumn(
                name: "CourtTimeSlotCourtID",
                table: "BookingTimeSlots");

            migrationBuilder.DropColumn(
                name: "CourtTimeSlotTimeSlotID",
                table: "BookingTimeSlots");

            migrationBuilder.RenameColumn(
                name: "TimeSlotID",
                table: "BookingTimeSlots",
                newName: "TimeSlotId");

            migrationBuilder.RenameIndex(
                name: "IX_BookingTimeSlots_TimeSlotID",
                table: "BookingTimeSlots",
                newName: "IX_BookingTimeSlots_TimeSlotId");

            migrationBuilder.AlterColumn<Guid>(
                name: "TimeSlotId",
                table: "BookingTimeSlots",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_BookingTimeSlots",
                table: "BookingTimeSlots",
                columns: new[] { "BookingId", "TimeSlotId" });

            migrationBuilder.AddForeignKey(
                name: "FK_BookingTimeSlots_TimeSlots_TimeSlotId",
                table: "BookingTimeSlots",
                column: "TimeSlotId",
                principalTable: "TimeSlots",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
