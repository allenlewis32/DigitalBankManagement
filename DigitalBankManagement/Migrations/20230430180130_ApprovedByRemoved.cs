using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalBankManagement.Migrations
{
    /// <inheritdoc />
    public partial class ApprovedByRemoved : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LoanApplications_Users_ApprovedBy",
                table: "LoanApplications");

            migrationBuilder.DropIndex(
                name: "IX_LoanApplications_ApprovedBy",
                table: "LoanApplications");

            migrationBuilder.DropColumn(
                name: "ApprovedBy",
                table: "LoanApplications");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ApprovedBy",
                table: "LoanApplications",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LoanApplications_ApprovedBy",
                table: "LoanApplications",
                column: "ApprovedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_LoanApplications_Users_ApprovedBy",
                table: "LoanApplications",
                column: "ApprovedBy",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
