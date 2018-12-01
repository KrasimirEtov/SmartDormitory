using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartDormitory.Data.Migrations
{
    public partial class RequiresMeToDoIt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "MeasureTypes",
                keyColumn: "Id",
                keyValue: "2524f6f3-5291-404b-b5e4-b24db2c0254a");

            migrationBuilder.DeleteData(
                table: "MeasureTypes",
                keyColumn: "Id",
                keyValue: "29d56a9e-ce59-4055-926c-f354621e7086");

            migrationBuilder.DeleteData(
                table: "MeasureTypes",
                keyColumn: "Id",
                keyValue: "697d3892-43be-4ab3-9ad9-57a3b2b168ea");

            migrationBuilder.DeleteData(
                table: "MeasureTypes",
                keyColumn: "Id",
                keyValue: "7df3c119-a73f-4cd8-b044-86d0b261e65a");

            migrationBuilder.DeleteData(
                table: "MeasureTypes",
                keyColumn: "Id",
                keyValue: "d27a77b3-1d45-4f40-8871-bc6aa4054686");

            migrationBuilder.InsertData(
                table: "MeasureTypes",
                columns: new[] { "Id", "CreatedOn", "DeletedOn", "IsDeleted", "MeasureUnit", "ModifiedOn", "SuitableSensorType" },
                values: new object[,]
                {
                    { "b5ed44ed-a38d-4dc8-ad8a-796e42d018da", new DateTime(2018, 12, 2, 0, 50, 56, 188, DateTimeKind.Local), null, false, "°C", null, "Temperature" },
                    { "d80a9fa0-36db-4565-b4b2-e17e46887127", new DateTime(2018, 12, 2, 0, 50, 56, 190, DateTimeKind.Local), null, false, "%", null, "Humidity" },
                    { "d60b36ae-20e9-4b7b-afc5-f4fead61cce7", new DateTime(2018, 12, 2, 0, 50, 56, 190, DateTimeKind.Local), null, false, "W", null, "Electric power consumtion" },
                    { "c81a1be2-dec4-47ce-b4d9-1000491b843f", new DateTime(2018, 12, 2, 0, 50, 56, 190, DateTimeKind.Local), null, false, "(true/false)", null, "Boolean switch (door/occupancy/etc)" },
                    { "6e6af1b4-2d82-4381-a5cf-03aed3f7664e", new DateTime(2018, 12, 2, 0, 50, 56, 190, DateTimeKind.Local), null, false, "dB", null, "Noise" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "MeasureTypes",
                keyColumn: "Id",
                keyValue: "6e6af1b4-2d82-4381-a5cf-03aed3f7664e");

            migrationBuilder.DeleteData(
                table: "MeasureTypes",
                keyColumn: "Id",
                keyValue: "b5ed44ed-a38d-4dc8-ad8a-796e42d018da");

            migrationBuilder.DeleteData(
                table: "MeasureTypes",
                keyColumn: "Id",
                keyValue: "c81a1be2-dec4-47ce-b4d9-1000491b843f");

            migrationBuilder.DeleteData(
                table: "MeasureTypes",
                keyColumn: "Id",
                keyValue: "d60b36ae-20e9-4b7b-afc5-f4fead61cce7");

            migrationBuilder.DeleteData(
                table: "MeasureTypes",
                keyColumn: "Id",
                keyValue: "d80a9fa0-36db-4565-b4b2-e17e46887127");

            migrationBuilder.InsertData(
                table: "MeasureTypes",
                columns: new[] { "Id", "CreatedOn", "DeletedOn", "IsDeleted", "MeasureUnit", "ModifiedOn", "SuitableSensorType" },
                values: new object[,]
                {
                    { "d27a77b3-1d45-4f40-8871-bc6aa4054686", new DateTime(2018, 11, 25, 23, 51, 1, 659, DateTimeKind.Local), null, false, "°C", null, "Temperature" },
                    { "7df3c119-a73f-4cd8-b044-86d0b261e65a", new DateTime(2018, 11, 25, 23, 51, 1, 661, DateTimeKind.Local), null, false, "%", null, "Humidity" },
                    { "2524f6f3-5291-404b-b5e4-b24db2c0254a", new DateTime(2018, 11, 25, 23, 51, 1, 661, DateTimeKind.Local), null, false, "W", null, "Electric power consumtion" },
                    { "29d56a9e-ce59-4055-926c-f354621e7086", new DateTime(2018, 11, 25, 23, 51, 1, 661, DateTimeKind.Local), null, false, "(true/false)", null, "Boolean switch (door/occupancy/etc)" },
                    { "697d3892-43be-4ab3-9ad9-57a3b2b168ea", new DateTime(2018, 11, 25, 23, 51, 1, 661, DateTimeKind.Local), null, false, "dB", null, "Noise" }
                });
        }
    }
}
