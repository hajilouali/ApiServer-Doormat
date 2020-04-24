using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class _98112011 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SanadAttachments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SanadID = table.Column<int>(nullable: false),
                    FileName = table.Column<string>(maxLength: 200, nullable: false),
                    Discription = table.Column<string>(maxLength: 600, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SanadAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SanadAttachments_Sanads_SanadID",
                        column: x => x.SanadID,
                        principalTable: "Sanads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SanadAttachments_SanadID",
                table: "SanadAttachments",
                column: "SanadID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SanadAttachments");
        }
    }
}
