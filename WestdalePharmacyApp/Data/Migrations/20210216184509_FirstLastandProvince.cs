using Microsoft.EntityFrameworkCore.Migrations;

namespace WestdalePharmacyApp.Data.Migrations
{
    public partial class FirstLastandProvince : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Province",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Province",
                table: "AspNetUsers");
        }
    }
}
