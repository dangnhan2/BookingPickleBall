using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PickleBall.Migrations
{
    /// <inheritdoc />
    public partial class DeletePropertyOfCourt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TimeSlots",
                keyColumn: "ID",
                keyValue: new Guid("2ff5a33e-c6f1-4765-b8d5-3b622679bac9"));

            migrationBuilder.DeleteData(
                table: "TimeSlots",
                keyColumn: "ID",
                keyValue: new Guid("436ef092-06fb-49df-b294-7f4acfd71d62"));

            migrationBuilder.DeleteData(
                table: "TimeSlots",
                keyColumn: "ID",
                keyValue: new Guid("4b2610bb-ab23-4ed3-ba1a-216ef9b27aee"));

            migrationBuilder.DeleteData(
                table: "TimeSlots",
                keyColumn: "ID",
                keyValue: new Guid("6545ccbc-3174-4c96-b9de-ba389f890a3c"));

            migrationBuilder.DeleteData(
                table: "TimeSlots",
                keyColumn: "ID",
                keyValue: new Guid("b5a96ad0-bdf1-4623-84bc-11a9e33ec84e"));

            migrationBuilder.DeleteData(
                table: "TimeSlots",
                keyColumn: "ID",
                keyValue: new Guid("b6d27507-de76-4043-8e2f-c0264db33989"));

            migrationBuilder.DeleteData(
                table: "TimeSlots",
                keyColumn: "ID",
                keyValue: new Guid("dc7cc6fc-9aaa-42e0-af2d-0b74c8176fe0"));

            migrationBuilder.DropColumn(
                name: "CourtTypeID",
                table: "Courts");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AddColumn<Guid>(
                name: "CourtTypeID",
                table: "Courts",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.InsertData(
                table: "TimeSlots",
                columns: new[] { "ID", "EndTime", "StartTime" },
                values: new object[,]
                {
                    { new Guid("2ff5a33e-c6f1-4765-b8d5-3b622679bac9"), new TimeOnly(20, 0, 0), new TimeOnly(18, 0, 0) },
                    { new Guid("436ef092-06fb-49df-b294-7f4acfd71d62"), new TimeOnly(10, 0, 0), new TimeOnly(8, 0, 0) },
                    { new Guid("4b2610bb-ab23-4ed3-ba1a-216ef9b27aee"), new TimeOnly(18, 0, 0), new TimeOnly(16, 0, 0) },
                    { new Guid("6545ccbc-3174-4c96-b9de-ba389f890a3c"), new TimeOnly(22, 0, 0), new TimeOnly(20, 0, 0) },
                    { new Guid("b5a96ad0-bdf1-4623-84bc-11a9e33ec84e"), new TimeOnly(16, 0, 0), new TimeOnly(14, 0, 0) },
                    { new Guid("b6d27507-de76-4043-8e2f-c0264db33989"), new TimeOnly(14, 0, 0), new TimeOnly(12, 0, 0) },
                    { new Guid("dc7cc6fc-9aaa-42e0-af2d-0b74c8176fe0"), new TimeOnly(12, 0, 0), new TimeOnly(10, 0, 0) }
                });
        }
    }
}
