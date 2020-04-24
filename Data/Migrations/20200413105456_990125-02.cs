using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class _99012502 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Tikets",
                newName: "Closed");

            migrationBuilder.AddColumn<DateTime>(
                name: "DataCreate",
                table: "Tikets",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DataModified",
                table: "Tikets",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<short>(
                name: "Department",
                table: "Tikets",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataCreate",
                table: "TiketContents",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DataModified",
                table: "TiketContents",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataCreate",
                table: "Tikets");

            migrationBuilder.DropColumn(
                name: "DataModified",
                table: "Tikets");

            migrationBuilder.DropColumn(
                name: "Department",
                table: "Tikets");

            migrationBuilder.DropColumn(
                name: "DataCreate",
                table: "TiketContents");

            migrationBuilder.DropColumn(
                name: "DataModified",
                table: "TiketContents");

            migrationBuilder.RenameColumn(
                name: "Closed",
                table: "Tikets",
                newName: "Status");
        }
    }
}
