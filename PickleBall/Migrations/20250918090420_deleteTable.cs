using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PickleBall.Migrations
{
    /// <inheritdoc />
    public partial class deleteTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "Bookings");

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpriedAt",
                table: "Bookings",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "TransactionId",
                table: "Bookings",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpriedAt",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "TransactionId",
                table: "Bookings");

            migrationBuilder.AddColumn<int>(
                name: "PaymentStatus",
                table: "Bookings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    BookingID = table.Column<Guid>(type: "uuid", nullable: false),
                    MethodPayment = table.Column<string>(type: "text", nullable: false),
                    OrderCode = table.Column<long>(type: "bigint", nullable: false),
                    PaidAmount = table.Column<int>(type: "integer", nullable: false),
                    PaidAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PaymentStatus = table.Column<int>(type: "integer", nullable: false),
                    TransactionID = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Payments_Bookings_BookingID",
                        column: x => x.BookingID,
                        principalTable: "Bookings",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_BookingID",
                table: "Payments",
                column: "BookingID",
                unique: true);
        }
    }
}
