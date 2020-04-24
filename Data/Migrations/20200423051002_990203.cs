using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class _990203 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserConnected",
                table: "Clients",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserConnected",
                table: "Clients");
        }
    }
}
