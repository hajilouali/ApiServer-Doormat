﻿// <auto-generated />
using System;
using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20191215111329_secendmigration")]
    partial class secendmigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.11-servicing-32099")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Entities.AccountingHeading", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AccountingType");

                    b.Property<string>("Discription")
                        .HasMaxLength(700);

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.ToTable("AccountingHeadings");
                });

            modelBuilder.Entity("Entities.Bank", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AccountingHeading_ID");

                    b.Property<string>("BankTitle")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.HasIndex("AccountingHeading_ID");

                    b.ToTable("Banks");
                });

            modelBuilder.Entity("Entities.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<int?>("ParentCategoryId");

                    b.HasKey("Id");

                    b.HasIndex("ParentCategoryId");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("Entities.Client", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AccountingHeading_ID");

                    b.Property<string>("ClientAddress")
                        .HasMaxLength(700);

                    b.Property<string>("ClientName")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.Property<string>("ClientPartnerName")
                        .HasMaxLength(200);

                    b.Property<string>("ClientPhone")
                        .HasMaxLength(50);

                    b.Property<int>("CodeEgtesadi");

                    b.Property<int>("CodeMeli");

                    b.Property<decimal>("DiscountPercent");

                    b.Property<double>("MaxCreditValue");

                    b.Property<int>("User_ID");

                    b.HasKey("Id");

                    b.HasIndex("AccountingHeading_ID");

                    b.HasIndex("User_ID");

                    b.ToTable("Clients");
                });

            modelBuilder.Entity("Entities.Expert", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Client_ID");

                    b.Property<DateTime>("DateTime");

                    b.Property<int>("ExpertCordition");

                    b.Property<int>("Facor_ID");

                    b.HasKey("Id");

                    b.HasIndex("Client_ID");

                    b.ToTable("Experts");
                });

            modelBuilder.Entity("Entities.ExpertHistory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("DateTime");

                    b.Property<int>("ExpertCordition");

                    b.Property<int>("Expert_ID");

                    b.Property<int>("User_ID");

                    b.HasKey("Id");

                    b.HasIndex("Expert_ID");

                    b.HasIndex("User_ID");

                    b.ToTable("ExpertHistories");
                });

            modelBuilder.Entity("Entities.Factor", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Client_ID");

                    b.Property<DateTime>("DateTime");

                    b.Property<double>("Discount");

                    b.Property<string>("FactorCodeView")
                        .HasMaxLength(200);

                    b.Property<double>("FactorPrice");

                    b.Property<double>("Taxes");

                    b.Property<double>("TotalPrice");

                    b.Property<int>("User_ID");

                    b.HasKey("Id");

                    b.HasIndex("Client_ID");

                    b.HasIndex("User_ID");

                    b.ToTable("Factors");
                });

            modelBuilder.Entity("Entities.FactorAttachment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Discription")
                        .HasMaxLength(600);

                    b.Property<int>("Facor_ID");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.HasIndex("Facor_ID");

                    b.ToTable("FactorAttachments");
                });

            modelBuilder.Entity("Entities.Manufacture", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ConditionManufacture");

                    b.Property<int>("Factor_ID");

                    b.Property<DateTime>("InDateTime");

                    b.HasKey("Id");

                    b.HasIndex("Factor_ID");

                    b.ToTable("Manufactures");
                });

            modelBuilder.Entity("Entities.ManufactureHistory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ConditionManufacture");

                    b.Property<DateTime>("DateTime");

                    b.Property<string>("Discription")
                        .HasMaxLength(600);

                    b.Property<int>("Manufacture_ID");

                    b.Property<int>("User_ID");

                    b.HasKey("Id");

                    b.HasIndex("Manufacture_ID");

                    b.HasIndex("User_ID");

                    b.ToTable("ManufactureHistories");
                });

            modelBuilder.Entity("Entities.Post", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("NEWSEQUENTIALID()");

                    b.Property<int>("AuthorId");

                    b.Property<int>("CategoryId");

                    b.Property<string>("Description")
                        .IsRequired();

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("CategoryId");

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("Entities.Product_Factor", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Factor_ID");

                    b.Property<int>("ProductAndService_ID");

                    b.HasKey("Id");

                    b.HasIndex("Factor_ID");

                    b.HasIndex("ProductAndService_ID");

                    b.ToTable("Product_Factors");
                });

            modelBuilder.Entity("Entities.ProductAndService", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ProductCode");

                    b.Property<string>("ProductName");

                    b.Property<int>("ProductType");

                    b.Property<int>("UnitPrice");

                    b.Property<int>("UnitType");

                    b.HasKey("Id");

                    b.ToTable("ProductAndServices");
                });

            modelBuilder.Entity("Entities.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Entities.Sanad", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AccountingHeading_ID");

                    b.Property<int>("Bedehkari");

                    b.Property<int>("Bestankari");

                    b.Property<string>("Comment")
                        .HasMaxLength(700);

                    b.Property<int>("SanadHeading_ID");

                    b.HasKey("Id");

                    b.HasIndex("AccountingHeading_ID");

                    b.HasIndex("SanadHeading_ID");

                    b.ToTable("Sanads");
                });

            modelBuilder.Entity("Entities.SanadHeading", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("DateTime");

                    b.Property<string>("Discription")
                        .HasMaxLength(700);

                    b.HasKey("Id");

                    b.ToTable("SanadHeadings");
                });

            modelBuilder.Entity("Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("Address")
                        .HasMaxLength(700);

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.Property<bool>("IsActive");

                    b.Property<DateTimeOffset?>("LastLoginDate");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<int>("RoleId");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<int>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<int>("UserId");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<int>", b =>
                {
                    b.Property<int>("UserId");

                    b.Property<int>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<int>", b =>
                {
                    b.Property<int>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("Entities.Bank", b =>
                {
                    b.HasOne("Entities.AccountingHeading", "AccountingHeading")
                        .WithMany("Banks")
                        .HasForeignKey("AccountingHeading_ID")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Entities.Category", b =>
                {
                    b.HasOne("Entities.Category", "ParentCategory")
                        .WithMany("ChildCategories")
                        .HasForeignKey("ParentCategoryId");
                });

            modelBuilder.Entity("Entities.Client", b =>
                {
                    b.HasOne("Entities.AccountingHeading", "AccountingHeading")
                        .WithMany("Clients")
                        .HasForeignKey("AccountingHeading_ID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Entities.User", "User")
                        .WithMany("Clients")
                        .HasForeignKey("User_ID")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Entities.Expert", b =>
                {
                    b.HasOne("Entities.Client", "Client")
                        .WithMany("Experts")
                        .HasForeignKey("Client_ID")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Entities.ExpertHistory", b =>
                {
                    b.HasOne("Entities.Expert", "Expert")
                        .WithMany("ExpertHistories")
                        .HasForeignKey("Expert_ID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Entities.User", "User")
                        .WithMany("ExpertHistories")
                        .HasForeignKey("User_ID")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Entities.Factor", b =>
                {
                    b.HasOne("Entities.Client", "Client")
                        .WithMany("Factors")
                        .HasForeignKey("Client_ID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Entities.User", "User")
                        .WithMany("Factors")
                        .HasForeignKey("User_ID")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Entities.FactorAttachment", b =>
                {
                    b.HasOne("Entities.Factor", "Factor")
                        .WithMany("FactorAttachment")
                        .HasForeignKey("Facor_ID")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Entities.Manufacture", b =>
                {
                    b.HasOne("Entities.Factor", "Factor")
                        .WithMany("Manufacture")
                        .HasForeignKey("Factor_ID")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Entities.ManufactureHistory", b =>
                {
                    b.HasOne("Entities.Manufacture", "Manufacture")
                        .WithMany("ManufactureHistories")
                        .HasForeignKey("Manufacture_ID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Entities.User", "User")
                        .WithMany("ManufactureHistories")
                        .HasForeignKey("User_ID")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Entities.Post", b =>
                {
                    b.HasOne("Entities.User", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Entities.Category", "Category")
                        .WithMany("Posts")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Entities.Product_Factor", b =>
                {
                    b.HasOne("Entities.Factor", "Factor")
                        .WithMany("Product_Factor")
                        .HasForeignKey("Factor_ID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Entities.ProductAndService", "ProductAndService")
                        .WithMany("Product_Factor")
                        .HasForeignKey("ProductAndService_ID")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Entities.Sanad", b =>
                {
                    b.HasOne("Entities.AccountingHeading", "AccountingHeading")
                        .WithMany("Sanads")
                        .HasForeignKey("AccountingHeading_ID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Entities.SanadHeading", "SanadHeading")
                        .WithMany("Sanads")
                        .HasForeignKey("SanadHeading_ID")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<int>", b =>
                {
                    b.HasOne("Entities.Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<int>", b =>
                {
                    b.HasOne("Entities.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<int>", b =>
                {
                    b.HasOne("Entities.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<int>", b =>
                {
                    b.HasOne("Entities.Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Entities.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<int>", b =>
                {
                    b.HasOne("Entities.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict);
                });
#pragma warning restore 612, 618
        }
    }
}