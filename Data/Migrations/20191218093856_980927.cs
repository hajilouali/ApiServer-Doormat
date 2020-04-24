using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class _980927 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FactorID",
                table: "SanadHeadings",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Product_Factors",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Unit",
                table: "Product_Factors",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Width",
                table: "Product_Factors",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "length",
                table: "Product_Factors",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalPrice",
                table: "Factors",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "Taxes",
                table: "Factors",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "FactorPrice",
                table: "Factors",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "Discount",
                table: "Factors",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AddColumn<int>(
                name: "FactorType",
                table: "Factors",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FactorID",
                table: "SanadHeadings");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Product_Factors");

            migrationBuilder.DropColumn(
                name: "Unit",
                table: "Product_Factors");

            migrationBuilder.DropColumn(
                name: "Width",
                table: "Product_Factors");

            migrationBuilder.DropColumn(
                name: "length",
                table: "Product_Factors");

            migrationBuilder.DropColumn(
                name: "FactorType",
                table: "Factors");

            migrationBuilder.AlterColumn<double>(
                name: "TotalPrice",
                table: "Factors",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<double>(
                name: "Taxes",
                table: "Factors",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<double>(
                name: "FactorPrice",
                table: "Factors",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<double>(
                name: "Discount",
                table: "Factors",
                nullable: false,
                oldClrType: typeof(decimal));
        }
    }
}
