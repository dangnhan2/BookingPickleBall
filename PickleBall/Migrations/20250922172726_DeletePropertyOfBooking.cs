using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PickleBall.Migrations
{
    /// <inheritdoc />
    public partial class DeletePropertyOfBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QRCodeUrl",
                table: "Bookings");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "QRCodeUrl",
                table: "Bookings",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
