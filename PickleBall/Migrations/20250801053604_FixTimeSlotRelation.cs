using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PickleBall.Migrations
{
    /// <inheritdoc />
    public partial class FixTimeSlotRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TimeSlots_Courts_CourtId",
                table: "TimeSlots");

            migrationBuilder.AlterColumn<Guid>(
                name: "CourtId",
                table: "TimeSlots",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_TimeSlots_Courts_CourtId",
                table: "TimeSlots",
                column: "CourtId",
                principalTable: "Courts",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TimeSlots_Courts_CourtId",
                table: "TimeSlots");

            migrationBuilder.AlterColumn<Guid>(
                name: "CourtId",
                table: "TimeSlots",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TimeSlots_Courts_CourtId",
                table: "TimeSlots",
                column: "CourtId",
                principalTable: "Courts",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
