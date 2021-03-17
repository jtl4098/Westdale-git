using Microsoft.EntityFrameworkCore.Migrations;

namespace WestdalePharmacyApp.Data.Migrations
{
    public partial class insurance_feb_28 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StreetAddress",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "AddressLine1",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AddressLine2",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InsuranceNumber",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddressLine1",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "AddressLine2",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "InsuranceNumber",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "StreetAddress",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
