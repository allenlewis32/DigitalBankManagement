﻿// <auto-generated />
using System;
using DigitalBankManagement.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DigitalBankManagement.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("DigitalBankManagement.Models.AccountModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(12, 2)");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("DigitalBankManagement.Models.BeneficiaryModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("BeneficiaryAccountId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BeneficiaryAccountId");

                    b.HasIndex("UserId");

                    b.ToTable("Beneficiaries");
                });

            modelBuilder.Entity("DigitalBankManagement.Models.CardModel", b =>
                {
                    b.Property<decimal>("Id")
                        .HasColumnType("decimal(16)");

                    b.Property<int?>("AccountId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Expiry")
                        .HasColumnType("datetime2");

                    b.Property<int>("Pin")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.ToTable("Cards");
                });

            modelBuilder.Entity("DigitalBankManagement.Models.FdAccountModel", b =>
                {
                    b.Property<int>("AccountId")
                        .HasColumnType("int");

                    b.Property<int>("Duration")
                        .HasColumnType("int");

                    b.Property<decimal>("InitialDeposit")
                        .HasColumnType("decimal(12, 2)");

                    b.HasKey("AccountId");

                    b.ToTable("FdAccounts");
                });

            modelBuilder.Entity("DigitalBankManagement.Models.InterestModel", b =>
                {
                    b.Property<decimal>("FD")
                        .HasColumnType("decimal(5, 2)");

                    b.Property<decimal>("Loan")
                        .HasColumnType("decimal(5, 2)");

                    b.Property<decimal>("RD")
                        .HasColumnType("decimal(5, 2)");

                    b.Property<decimal>("Savings")
                        .HasColumnType("decimal(5, 2)");

                    b.ToTable("Interests");
                });

            modelBuilder.Entity("DigitalBankManagement.Models.LoanApplicationModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(12, 2)");

                    b.Property<int?>("DebitFrom")
                        .HasColumnType("int");

                    b.Property<int>("Duration")
                        .HasColumnType("int");

                    b.Property<int?>("LoanId")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("DebitFrom");

                    b.HasIndex("LoanId");

                    b.HasIndex("UserId");

                    b.ToTable("LoanApplications");
                });

            modelBuilder.Entity("DigitalBankManagement.Models.LoanModel", b =>
                {
                    b.Property<int>("AccountId")
                        .HasColumnType("int");

                    b.Property<int?>("DebitFrom")
                        .HasColumnType("int");

                    b.Property<int>("Duration")
                        .HasColumnType("int");

                    b.Property<decimal>("Emi")
                        .HasColumnType("decimal(12, 2)");

                    b.HasKey("AccountId");

                    b.HasIndex("DebitFrom");

                    b.ToTable("Loans");
                });

            modelBuilder.Entity("DigitalBankManagement.Models.RdAccountModel", b =>
                {
                    b.Property<int>("AccountId")
                        .HasColumnType("int");

                    b.Property<int?>("DebitFrom")
                        .HasColumnType("int");

                    b.Property<int>("Duration")
                        .HasColumnType("int");

                    b.Property<decimal>("MonthlyDeposit")
                        .HasColumnType("decimal(12, 2)");

                    b.HasKey("AccountId");

                    b.HasIndex("DebitFrom");

                    b.ToTable("RdAccounts");
                });

            modelBuilder.Entity("DigitalBankManagement.Models.RegisterModel", b =>
                {
                    b.Property<string>("ConfirmPassword")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SessionId")
                        .HasColumnType("nvarchar(max)");

                    b.ToTable("RegisterModel");
                });

            modelBuilder.Entity("DigitalBankManagement.Models.RoleModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("DigitalBankManagement.Models.SessionModel", b =>
                {
                    b.Property<string>("SessionId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("LastUsed")
                        .HasColumnType("datetime2");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("SessionId");

                    b.HasIndex("UserId");

                    b.ToTable("Sessions");
                });

            modelBuilder.Entity("DigitalBankManagement.Models.TransactionModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(12, 2)");

                    b.Property<int?>("FromAccountId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Time")
                        .HasColumnType("datetime2");

                    b.Property<int?>("ToAccountId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("FromAccountId");

                    b.HasIndex("ToAccountId");

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("DigitalBankManagement.Models.UserModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("RoleId")
                        .HasColumnType("int");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("RoleId");

                    b.HasIndex("UserName")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("DigitalBankManagement.Models.AccountModel", b =>
                {
                    b.HasOne("DigitalBankManagement.Models.UserModel", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("DigitalBankManagement.Models.BeneficiaryModel", b =>
                {
                    b.HasOne("DigitalBankManagement.Models.AccountModel", "BeneficiaryAccount")
                        .WithMany()
                        .HasForeignKey("BeneficiaryAccountId");

                    b.HasOne("DigitalBankManagement.Models.UserModel", "User")
                        .WithMany()
                        .HasForeignKey("UserId");

                    b.Navigation("BeneficiaryAccount");

                    b.Navigation("User");
                });

            modelBuilder.Entity("DigitalBankManagement.Models.CardModel", b =>
                {
                    b.HasOne("DigitalBankManagement.Models.AccountModel", "Account")
                        .WithMany()
                        .HasForeignKey("AccountId");

                    b.Navigation("Account");
                });

            modelBuilder.Entity("DigitalBankManagement.Models.FdAccountModel", b =>
                {
                    b.HasOne("DigitalBankManagement.Models.AccountModel", "Account")
                        .WithMany()
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");
                });

            modelBuilder.Entity("DigitalBankManagement.Models.LoanApplicationModel", b =>
                {
                    b.HasOne("DigitalBankManagement.Models.AccountModel", "DebitAccount")
                        .WithMany()
                        .HasForeignKey("DebitFrom");

                    b.HasOne("DigitalBankManagement.Models.AccountModel", "LoanAccount")
                        .WithMany()
                        .HasForeignKey("LoanId");

                    b.HasOne("DigitalBankManagement.Models.UserModel", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DebitAccount");

                    b.Navigation("LoanAccount");

                    b.Navigation("User");
                });

            modelBuilder.Entity("DigitalBankManagement.Models.LoanModel", b =>
                {
                    b.HasOne("DigitalBankManagement.Models.AccountModel", "Account")
                        .WithMany()
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DigitalBankManagement.Models.AccountModel", "DebitAccount")
                        .WithMany()
                        .HasForeignKey("DebitFrom");

                    b.Navigation("Account");

                    b.Navigation("DebitAccount");
                });

            modelBuilder.Entity("DigitalBankManagement.Models.RdAccountModel", b =>
                {
                    b.HasOne("DigitalBankManagement.Models.AccountModel", "Account")
                        .WithMany()
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DigitalBankManagement.Models.AccountModel", "DebitAccount")
                        .WithMany()
                        .HasForeignKey("DebitFrom");

                    b.Navigation("Account");

                    b.Navigation("DebitAccount");
                });

            modelBuilder.Entity("DigitalBankManagement.Models.SessionModel", b =>
                {
                    b.HasOne("DigitalBankManagement.Models.UserModel", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("DigitalBankManagement.Models.TransactionModel", b =>
                {
                    b.HasOne("DigitalBankManagement.Models.AccountModel", "FromAccount")
                        .WithMany()
                        .HasForeignKey("FromAccountId");

                    b.HasOne("DigitalBankManagement.Models.AccountModel", "ToAccount")
                        .WithMany()
                        .HasForeignKey("ToAccountId");

                    b.Navigation("FromAccount");

                    b.Navigation("ToAccount");
                });

            modelBuilder.Entity("DigitalBankManagement.Models.UserModel", b =>
                {
                    b.HasOne("DigitalBankManagement.Models.RoleModel", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");
                });
#pragma warning restore 612, 618
        }
    }
}
