using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PickleBall.Migrations
{
    /// <inheritdoc />
    public partial class FixCourtRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_TimeSlots_TimeSlotID",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_TimeSlotID",
                table: "Bookings");

            migrationBuilder.DeleteData(
                table: "TimeSlots",
                keyColumn: "ID",
                keyValue: new Guid("24e9923b-42cb-4f50-90c6-9c3e536b3083"));

            migrationBuilder.DeleteData(
                table: "TimeSlots",
                keyColumn: "ID",
                keyValue: new Guid("416f2e3b-e150-413c-8313-36ec2db031fb"));

            migrationBuilder.DeleteData(
                table: "TimeSlots",
                keyColumn: "ID",
                keyValue: new Guid("453e9de8-8d47-485e-b3fd-29b98c4bf3b3"));

            migrationBuilder.DeleteData(
                table: "TimeSlots",
                keyColumn: "ID",
                keyValue: new Guid("5aa53b67-60dc-4676-b6b2-f05b6e23edbb"));

            migrationBuilder.DeleteData(
                table: "TimeSlots",
                keyColumn: "ID",
                keyValue: new Guid("72a1ed19-47dd-4c19-adfa-cd175b8af688"));

            migrationBuilder.DeleteData(
                table: "TimeSlots",
                keyColumn: "ID",
                keyValue: new Guid("c335bbce-cbc5-40e6-8e08-099aae56a95e"));

            migrationBuilder.DeleteData(
                table: "TimeSlots",
                keyColumn: "ID",
                keyValue: new Guid("f5a6b30e-57e3-42be-ab42-ee4dfa8e181f"));

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "TimeSlotID",
                table: "Bookings");

            migrationBuilder.AddColumn<Guid>(
                name: "CourtId",
                table: "TimeSlots",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "BookingTimeSlots",
                columns: table => new
                {
                    BookingId = table.Column<Guid>(type: "uuid", nullable: false),
                    TimeSlotId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingTimeSlots", x => new { x.BookingId, x.TimeSlotId });
                    table.ForeignKey(
                        name: "FK_BookingTimeSlots_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookingTimeSlots_TimeSlots_TimeSlotId",
                        column: x => x.TimeSlotId,
                        principalTable: "TimeSlots",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TimeSlots_CourtId",
                table: "TimeSlots",
                column: "CourtId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingTimeSlots_TimeSlotId",
                table: "BookingTimeSlots",
                column: "TimeSlotId");

            migrationBuilder.AddForeignKey(
                name: "FK_TimeSlots_Courts_CourtId",
                table: "TimeSlots",
                column: "CourtId",
                principalTable: "Courts",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TimeSlots_Courts_CourtId",
                table: "TimeSlots");

            migrationBuilder.DropTable(
                name: "BookingTimeSlots");

            migrationBuilder.DropIndex(
                name: "IX_TimeSlots_CourtId",
                table: "TimeSlots");

            migrationBuilder.DropColumn(
                name: "CourtId",
                table: "TimeSlots");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Payments",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Bookings",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "TimeSlotID",
                table: "Bookings",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.InsertData(
                table: "TimeSlots",
                columns: new[] { "ID", "EndTime", "StartTime" },
                values: new object[,]
                {
                    { new Guid("24e9923b-42cb-4f50-90c6-9c3e536b3083"), new TimeOnly(18, 0, 0), new TimeOnly(16, 0, 0) },
                    { new Guid("416f2e3b-e150-413c-8313-36ec2db031fb"), new TimeOnly(10, 0, 0), new TimeOnly(8, 0, 0) },
                    { new Guid("453e9de8-8d47-485e-b3fd-29b98c4bf3b3"), new TimeOnly(14, 0, 0), new TimeOnly(12, 0, 0) },
                    { new Guid("5aa53b67-60dc-4676-b6b2-f05b6e23edbb"), new TimeOnly(20, 0, 0), new TimeOnly(18, 0, 0) },
                    { new Guid("72a1ed19-47dd-4c19-adfa-cd175b8af688"), new TimeOnly(22, 0, 0), new TimeOnly(20, 0, 0) },
                    { new Guid("c335bbce-cbc5-40e6-8e08-099aae56a95e"), new TimeOnly(16, 0, 0), new TimeOnly(14, 0, 0) },
                    { new Guid("f5a6b30e-57e3-42be-ab42-ee4dfa8e181f"), new TimeOnly(12, 0, 0), new TimeOnly(10, 0, 0) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_TimeSlotID",
                table: "Bookings",
                column: "TimeSlotID");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_TimeSlots_TimeSlotID",
                table: "Bookings",
                column: "TimeSlotID",
                principalTable: "TimeSlots",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
