using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalBankManagement.Migrations
{
    /// <inheritdoc />
    public partial class loanModelAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Loans",
                columns: table => new
                {
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    Emi = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    DebitFrom = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Loans", x => x.AccountId);
                    table.ForeignKey(
                        name: "FK_Loans_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Loans_Accounts_DebitFrom",
                        column: x => x.DebitFrom,
                        principalTable: "Accounts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Loans_DebitFrom",
                table: "Loans",
                column: "DebitFrom");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Loans");
        }
    }
}
