using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class _990212 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserID",
                table: "TiketContents",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TiketContents_UserID",
                table: "TiketContents",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_TiketContents_AspNetUsers_UserID",
                table: "TiketContents",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TiketContents_AspNetUsers_UserID",
                table: "TiketContents");

            migrationBuilder.DropIndex(
                name: "IX_TiketContents_UserID",
                table: "TiketContents");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "TiketContents");
        }
    }
}
