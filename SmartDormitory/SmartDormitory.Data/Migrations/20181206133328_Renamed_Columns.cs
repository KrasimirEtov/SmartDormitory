using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartDormitory.Data.Migrations
{
    public partial class Renamed_Columns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sensors_AspNetUsers_OwnerId",
                table: "Sensors");

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

            migrationBuilder.RenameColumn(
                name: "UserPollingInterval",
                table: "Sensors",
                newName: "PollingInterval");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "Sensors",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "AlarmMinRangeValue",
                table: "Sensors",
                newName: "MinRangeValue");

            migrationBuilder.RenameColumn(
                name: "AlarmMaxRangeValue",
                table: "Sensors",
                newName: "MaxRangeValue");

            migrationBuilder.RenameIndex(
                name: "IX_Sensors_OwnerId",
                table: "Sensors",
                newName: "IX_Sensors_UserId");

            migrationBuilder.InsertData(
                table: "MeasureTypes",
                columns: new[] { "Id", "CreatedOn", "DeletedOn", "IsDeleted", "MeasureUnit", "ModifiedOn", "SuitableSensorType" },
                values: new object[,]
                {
                    { "6fb4e583-ec91-4a9f-bf72-8c4ad3b992a3", new DateTime(2018, 12, 6, 15, 33, 27, 860, DateTimeKind.Local), null, false, "°C", null, "Temperature" },
                    { "15c82309-746f-4972-a224-1e9a8df08296", new DateTime(2018, 12, 6, 15, 33, 27, 863, DateTimeKind.Local), null, false, "%", null, "Humidity" },
                    { "e2eb0dd9-4f91-46f6-9be4-d53c302c7327", new DateTime(2018, 12, 6, 15, 33, 27, 863, DateTimeKind.Local), null, false, "W", null, "Electric power consumtion" },
                    { "cf66908e-39b0-421e-9806-ecfd5e2ca1e3", new DateTime(2018, 12, 6, 15, 33, 27, 863, DateTimeKind.Local), null, false, "(true/false)", null, "Boolean switch (door/occupancy/etc)" },
                    { "674f9f64-12c6-4abc-bb8c-cecbf440c2ce", new DateTime(2018, 12, 6, 15, 33, 27, 863, DateTimeKind.Local), null, false, "dB", null, "Noise" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Sensors_AspNetUsers_UserId",
                table: "Sensors",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sensors_AspNetUsers_UserId",
                table: "Sensors");

            migrationBuilder.DeleteData(
                table: "MeasureTypes",
                keyColumn: "Id",
                keyValue: "15c82309-746f-4972-a224-1e9a8df08296");

            migrationBuilder.DeleteData(
                table: "MeasureTypes",
                keyColumn: "Id",
                keyValue: "674f9f64-12c6-4abc-bb8c-cecbf440c2ce");

            migrationBuilder.DeleteData(
                table: "MeasureTypes",
                keyColumn: "Id",
                keyValue: "6fb4e583-ec91-4a9f-bf72-8c4ad3b992a3");

            migrationBuilder.DeleteData(
                table: "MeasureTypes",
                keyColumn: "Id",
                keyValue: "cf66908e-39b0-421e-9806-ecfd5e2ca1e3");

            migrationBuilder.DeleteData(
                table: "MeasureTypes",
                keyColumn: "Id",
                keyValue: "e2eb0dd9-4f91-46f6-9be4-d53c302c7327");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Sensors",
                newName: "OwnerId");

            migrationBuilder.RenameColumn(
                name: "PollingInterval",
                table: "Sensors",
                newName: "UserPollingInterval");

            migrationBuilder.RenameColumn(
                name: "MinRangeValue",
                table: "Sensors",
                newName: "AlarmMinRangeValue");

            migrationBuilder.RenameColumn(
                name: "MaxRangeValue",
                table: "Sensors",
                newName: "AlarmMaxRangeValue");

            migrationBuilder.RenameIndex(
                name: "IX_Sensors_UserId",
                table: "Sensors",
                newName: "IX_Sensors_OwnerId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Sensors_AspNetUsers_OwnerId",
                table: "Sensors",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
