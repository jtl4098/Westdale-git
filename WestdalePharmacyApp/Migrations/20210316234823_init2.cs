using Microsoft.EntityFrameworkCore.Migrations;

namespace WestdalePharmacyApp.Migrations
{
    public partial class init2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_AspNetUsers_From_UserId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_AspNetUsers_To_UserId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_From_UserId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_To_UserId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "From_UserId",
                table: "Messages");

            migrationBuilder.AlterColumn<string>(
                name: "To_UserId",
                table: "Messages",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "From_UserEmail",
                table: "Messages",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Messages",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_UserId",
                table: "Messages",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_AspNetUsers_UserId",
                table: "Messages",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_AspNetUsers_UserId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_UserId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "From_UserEmail",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Messages");

            migrationBuilder.AlterColumn<string>(
                name: "To_UserId",
                table: "Messages",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "From_UserId",
                table: "Messages",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_From_UserId",
                table: "Messages",
                column: "From_UserId",
                unique: true,
                filter: "[From_UserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_To_UserId",
                table: "Messages",
                column: "To_UserId",
                unique: true,
                filter: "[To_UserId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_AspNetUsers_From_UserId",
                table: "Messages",
                column: "From_UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_AspNetUsers_To_UserId",
                table: "Messages",
                column: "To_UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
