using Microsoft.EntityFrameworkCore.Migrations;

namespace WestdalePharmacyApp.Data.Migrations
{
    public partial class fixedgenderprovince : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Gender",
                table: "AspNetUsers",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Gender",
                table: "AspNetUsers",
                type: "nvarchar(1)",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
