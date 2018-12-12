using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartDormitory.Data.Migrations
{
    public partial class Add_User_IsLocked_Property : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsLocked",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsLocked",
                table: "AspNetUsers");
        }
    }
}
