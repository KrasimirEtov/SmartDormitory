using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartDormitory.Data.Migrations
{
    public partial class Addnotifications : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "MeasureTypes",
                keyColumn: "Id",
                keyValue: "0bb1474d-31c0-42c1-9c6f-a06f335bea34");

            migrationBuilder.DeleteData(
                table: "MeasureTypes",
                keyColumn: "Id",
                keyValue: "36afaec6-efcc-4a34-acf9-906d35468a7f");

            migrationBuilder.DeleteData(
                table: "MeasureTypes",
                keyColumn: "Id",
                keyValue: "62e85dbc-39d1-458b-813b-4c98cf0eefd3");

            migrationBuilder.DeleteData(
                table: "MeasureTypes",
                keyColumn: "Id",
                keyValue: "adc385ea-29cf-48b5-bfa6-e0c226a69de8");

            migrationBuilder.DeleteData(
                table: "MeasureTypes",
                keyColumn: "Id",
                keyValue: "d77f06d4-587e-4e40-a240-1c1ab5e1ba1a");

            migrationBuilder.AddColumn<float>(
                name: "CurrentValue",
                table: "Sensors",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdateOn",
                table: "Sensors",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeletedOn = table.Column<DateTime>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: true),
                    ModifiedOn = table.Column<DateTime>(nullable: true),
                    Message = table.Column<string>(nullable: true),
                    ReceiverId = table.Column<string>(nullable: true),
                    Seen = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notification_AspNetUsers_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "MeasureTypes",
                columns: new[] { "Id", "CreatedOn", "DeletedOn", "IsDeleted", "MeasureUnit", "ModifiedOn", "SuitableSensorType" },
                values: new object[,]
                {
                    { "022219d6-3cec-473f-8b77-99c454da45d4", new DateTime(2018, 12, 8, 19, 57, 31, 971, DateTimeKind.Local).AddTicks(1335), null, false, "°C", null, "Temperature" },
                    { "d9d7d76c-ec0a-47d3-a01f-d230e656b461", new DateTime(2018, 12, 8, 19, 57, 31, 973, DateTimeKind.Local).AddTicks(1436), null, false, "%", null, "Humidity" },
                    { "8824cfc9-6dc5-45c9-a902-8f7307852c0c", new DateTime(2018, 12, 8, 19, 57, 31, 973, DateTimeKind.Local).AddTicks(1460), null, false, "W", null, "Electric power consumtion" },
                    { "011d139f-7b86-4fc3-b97f-ae859b1f9422", new DateTime(2018, 12, 8, 19, 57, 31, 973, DateTimeKind.Local).AddTicks(1468), null, false, "(true/false)", null, "Boolean switch (door/occupancy/etc)" },
                    { "4b4a42c5-f227-4b85-991e-83eb68bbeccd", new DateTime(2018, 12, 8, 19, 57, 31, 973, DateTimeKind.Local).AddTicks(1472), null, false, "dB", null, "Noise" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notification_ReceiverId",
                table: "Notification",
                column: "ReceiverId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.DeleteData(
                table: "MeasureTypes",
                keyColumn: "Id",
                keyValue: "011d139f-7b86-4fc3-b97f-ae859b1f9422");

            migrationBuilder.DeleteData(
                table: "MeasureTypes",
                keyColumn: "Id",
                keyValue: "022219d6-3cec-473f-8b77-99c454da45d4");

            migrationBuilder.DeleteData(
                table: "MeasureTypes",
                keyColumn: "Id",
                keyValue: "4b4a42c5-f227-4b85-991e-83eb68bbeccd");

            migrationBuilder.DeleteData(
                table: "MeasureTypes",
                keyColumn: "Id",
                keyValue: "8824cfc9-6dc5-45c9-a902-8f7307852c0c");

            migrationBuilder.DeleteData(
                table: "MeasureTypes",
                keyColumn: "Id",
                keyValue: "d9d7d76c-ec0a-47d3-a01f-d230e656b461");

            migrationBuilder.DropColumn(
                name: "CurrentValue",
                table: "Sensors");

            migrationBuilder.DropColumn(
                name: "LastUpdateOn",
                table: "Sensors");

            migrationBuilder.InsertData(
                table: "MeasureTypes",
                columns: new[] { "Id", "CreatedOn", "DeletedOn", "IsDeleted", "MeasureUnit", "ModifiedOn", "SuitableSensorType" },
                values: new object[,]
                {
                    { "62e85dbc-39d1-458b-813b-4c98cf0eefd3", new DateTime(2018, 12, 8, 6, 3, 36, 79, DateTimeKind.Local).AddTicks(7452), null, false, "°C", null, "Temperature" },
                    { "adc385ea-29cf-48b5-bfa6-e0c226a69de8", new DateTime(2018, 12, 8, 6, 3, 36, 83, DateTimeKind.Local).AddTicks(826), null, false, "%", null, "Humidity" },
                    { "36afaec6-efcc-4a34-acf9-906d35468a7f", new DateTime(2018, 12, 8, 6, 3, 36, 83, DateTimeKind.Local).AddTicks(847), null, false, "W", null, "Electric power consumtion" },
                    { "0bb1474d-31c0-42c1-9c6f-a06f335bea34", new DateTime(2018, 12, 8, 6, 3, 36, 83, DateTimeKind.Local).AddTicks(911), null, false, "(true/false)", null, "Boolean switch (door/occupancy/etc)" },
                    { "d77f06d4-587e-4e40-a240-1c1ab5e1ba1a", new DateTime(2018, 12, 8, 6, 3, 36, 83, DateTimeKind.Local).AddTicks(920), null, false, "dB", null, "Noise" }
                });
        }
    }
}
