using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class _981101 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RowDiscription",
                table: "Product_Factors",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discription",
                table: "Factors",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowDiscription",
                table: "Product_Factors");

            migrationBuilder.DropColumn(
                name: "Discription",
                table: "Factors");
        }
    }
}
