using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalBankManagement.Migrations
{
    /// <inheritdoc />
    public partial class InterestModelAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Interests",
                columns: table => new
                {
                    Savings = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    FD = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    RD = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Loan = table.Column<decimal>(type: "decimal(5,2)", nullable: false)
                },
                constraints: table =>
                {
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Interests");
        }
    }
}
