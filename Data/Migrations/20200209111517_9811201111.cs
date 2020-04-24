using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class _9811201111 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Price",
                table: "Product_Factors",
                nullable: false,
                oldClrType: typeof(decimal));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Product_Factors",
                nullable: false,
                oldClrType: typeof(double));
        }
    }
}
