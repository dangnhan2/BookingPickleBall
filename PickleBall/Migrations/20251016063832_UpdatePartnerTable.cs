using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PickleBall.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePartnerTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PartnerId",
                table: "Bookings",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "PayOSApiKey",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PayOSCheckSumKey",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PayOSClientId",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_PartnerId",
                table: "Bookings",
                column: "PartnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_AspNetUsers_PartnerId",
                table: "Bookings",
                column: "PartnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_AspNetUsers_PartnerId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_PartnerId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "PartnerId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "PayOSApiKey",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PayOSCheckSumKey",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PayOSClientId",
                table: "AspNetUsers");
        }
    }
}
