using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class _980930 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Facor_ID",
                table: "Experts");

            migrationBuilder.AddColumn<int>(
                name: "Facor_ID",
                table: "ExpertHistories",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ExpertHistories_Facor_ID",
                table: "ExpertHistories",
                column: "Facor_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_ExpertHistories_Factors_Facor_ID",
                table: "ExpertHistories",
                column: "Facor_ID",
                principalTable: "Factors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExpertHistories_Factors_Facor_ID",
                table: "ExpertHistories");

            migrationBuilder.DropIndex(
                name: "IX_ExpertHistories_Facor_ID",
                table: "ExpertHistories");

            migrationBuilder.DropColumn(
                name: "Facor_ID",
                table: "ExpertHistories");

            migrationBuilder.AddColumn<int>(
                name: "Facor_ID",
                table: "Experts",
                nullable: false,
                defaultValue: 0);
        }
    }
}
