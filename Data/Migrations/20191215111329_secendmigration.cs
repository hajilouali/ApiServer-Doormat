using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class secendmigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Age",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "AspNetUsers",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "AspNetUsers",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "AspNetUsers",
                maxLength: 700,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AccountingHeadings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(maxLength: 200, nullable: false),
                    AccountingType = table.Column<int>(nullable: false),
                    Discription = table.Column<string>(maxLength: 700, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountingHeadings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductAndServices",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProductName = table.Column<string>(nullable: true),
                    ProductCode = table.Column<string>(nullable: true),
                    UnitType = table.Column<int>(nullable: false),
                    UnitPrice = table.Column<int>(nullable: false),
                    ProductType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductAndServices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SanadHeadings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateTime = table.Column<DateTime>(nullable: false),
                    Discription = table.Column<string>(maxLength: 700, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SanadHeadings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Banks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BankTitle = table.Column<string>(maxLength: 200, nullable: false),
                    AccountingHeading_ID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Banks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Banks_AccountingHeadings_AccountingHeading_ID",
                        column: x => x.AccountingHeading_ID,
                        principalTable: "AccountingHeadings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ClientName = table.Column<string>(maxLength: 200, nullable: false),
                    ClientAddress = table.Column<string>(maxLength: 700, nullable: true),
                    ClientPhone = table.Column<string>(maxLength: 50, nullable: true),
                    CodeMeli = table.Column<int>(nullable: false),
                    CodeEgtesadi = table.Column<int>(nullable: false),
                    DiscountPercent = table.Column<decimal>(nullable: false),
                    AccountingHeading_ID = table.Column<int>(nullable: false),
                    User_ID = table.Column<int>(nullable: false),
                    MaxCreditValue = table.Column<double>(nullable: false),
                    ClientPartnerName = table.Column<string>(maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clients_AccountingHeadings_AccountingHeading_ID",
                        column: x => x.AccountingHeading_ID,
                        principalTable: "AccountingHeadings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Clients_AspNetUsers_User_ID",
                        column: x => x.User_ID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Sanads",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccountingHeading_ID = table.Column<int>(nullable: false),
                    SanadHeading_ID = table.Column<int>(nullable: false),
                    Bedehkari = table.Column<int>(nullable: false),
                    Bestankari = table.Column<int>(nullable: false),
                    Comment = table.Column<string>(maxLength: 700, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sanads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sanads_AccountingHeadings_AccountingHeading_ID",
                        column: x => x.AccountingHeading_ID,
                        principalTable: "AccountingHeadings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Sanads_SanadHeadings_SanadHeading_ID",
                        column: x => x.SanadHeading_ID,
                        principalTable: "SanadHeadings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Experts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateTime = table.Column<DateTime>(nullable: false),
                    ExpertCordition = table.Column<int>(nullable: false),
                    Client_ID = table.Column<int>(nullable: false),
                    Facor_ID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Experts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Experts_Clients_Client_ID",
                        column: x => x.Client_ID,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Factors",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateTime = table.Column<DateTime>(nullable: false),
                    TotalPrice = table.Column<double>(nullable: false),
                    Taxes = table.Column<double>(nullable: false),
                    Discount = table.Column<double>(nullable: false),
                    FactorPrice = table.Column<double>(nullable: false),
                    User_ID = table.Column<int>(nullable: false),
                    Client_ID = table.Column<int>(nullable: false),
                    FactorCodeView = table.Column<string>(maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Factors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Factors_Clients_Client_ID",
                        column: x => x.Client_ID,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Factors_AspNetUsers_User_ID",
                        column: x => x.User_ID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExpertHistories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ExpertCordition = table.Column<int>(nullable: false),
                    DateTime = table.Column<DateTime>(nullable: false),
                    User_ID = table.Column<int>(nullable: false),
                    Expert_ID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpertHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExpertHistories_Experts_Expert_ID",
                        column: x => x.Expert_ID,
                        principalTable: "Experts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExpertHistories_AspNetUsers_User_ID",
                        column: x => x.User_ID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FactorAttachments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Facor_ID = table.Column<int>(nullable: false),
                    FileName = table.Column<string>(maxLength: 200, nullable: false),
                    Discription = table.Column<string>(maxLength: 600, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FactorAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FactorAttachments_Factors_Facor_ID",
                        column: x => x.Facor_ID,
                        principalTable: "Factors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Manufactures",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Factor_ID = table.Column<int>(nullable: false),
                    InDateTime = table.Column<DateTime>(nullable: false),
                    ConditionManufacture = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Manufactures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Manufactures_Factors_Factor_ID",
                        column: x => x.Factor_ID,
                        principalTable: "Factors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Product_Factors",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProductAndService_ID = table.Column<int>(nullable: false),
                    Factor_ID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product_Factors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Product_Factors_Factors_Factor_ID",
                        column: x => x.Factor_ID,
                        principalTable: "Factors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Product_Factors_ProductAndServices_ProductAndService_ID",
                        column: x => x.ProductAndService_ID,
                        principalTable: "ProductAndServices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ManufactureHistories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    User_ID = table.Column<int>(nullable: false),
                    Manufacture_ID = table.Column<int>(nullable: false),
                    DateTime = table.Column<DateTime>(nullable: false),
                    ConditionManufacture = table.Column<int>(nullable: false),
                    Discription = table.Column<string>(maxLength: 600, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManufactureHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ManufactureHistories_Manufactures_Manufacture_ID",
                        column: x => x.Manufacture_ID,
                        principalTable: "Manufactures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ManufactureHistories_AspNetUsers_User_ID",
                        column: x => x.User_ID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Banks_AccountingHeading_ID",
                table: "Banks",
                column: "AccountingHeading_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_AccountingHeading_ID",
                table: "Clients",
                column: "AccountingHeading_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_User_ID",
                table: "Clients",
                column: "User_ID");

            migrationBuilder.CreateIndex(
                name: "IX_ExpertHistories_Expert_ID",
                table: "ExpertHistories",
                column: "Expert_ID");

            migrationBuilder.CreateIndex(
                name: "IX_ExpertHistories_User_ID",
                table: "ExpertHistories",
                column: "User_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Experts_Client_ID",
                table: "Experts",
                column: "Client_ID");

            migrationBuilder.CreateIndex(
                name: "IX_FactorAttachments_Facor_ID",
                table: "FactorAttachments",
                column: "Facor_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Factors_Client_ID",
                table: "Factors",
                column: "Client_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Factors_User_ID",
                table: "Factors",
                column: "User_ID");

            migrationBuilder.CreateIndex(
                name: "IX_ManufactureHistories_Manufacture_ID",
                table: "ManufactureHistories",
                column: "Manufacture_ID");

            migrationBuilder.CreateIndex(
                name: "IX_ManufactureHistories_User_ID",
                table: "ManufactureHistories",
                column: "User_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Manufactures_Factor_ID",
                table: "Manufactures",
                column: "Factor_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Product_Factors_Factor_ID",
                table: "Product_Factors",
                column: "Factor_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Product_Factors_ProductAndService_ID",
                table: "Product_Factors",
                column: "ProductAndService_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Sanads_AccountingHeading_ID",
                table: "Sanads",
                column: "AccountingHeading_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Sanads_SanadHeading_ID",
                table: "Sanads",
                column: "SanadHeading_ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Banks");

            migrationBuilder.DropTable(
                name: "ExpertHistories");

            migrationBuilder.DropTable(
                name: "FactorAttachments");

            migrationBuilder.DropTable(
                name: "ManufactureHistories");

            migrationBuilder.DropTable(
                name: "Product_Factors");

            migrationBuilder.DropTable(
                name: "Sanads");

            migrationBuilder.DropTable(
                name: "Experts");

            migrationBuilder.DropTable(
                name: "Manufactures");

            migrationBuilder.DropTable(
                name: "ProductAndServices");

            migrationBuilder.DropTable(
                name: "SanadHeadings");

            migrationBuilder.DropTable(
                name: "Factors");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "AccountingHeadings");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "AspNetUsers",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "AspNetUsers",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 200);

            migrationBuilder.AddColumn<int>(
                name: "Age",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Gender",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: 0);
        }
    }
}
