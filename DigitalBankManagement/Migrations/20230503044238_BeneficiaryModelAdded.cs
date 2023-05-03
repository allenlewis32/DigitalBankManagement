using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalBankManagement.Migrations
{
    /// <inheritdoc />
    public partial class BeneficiaryModelAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Beneficiaries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BeneficiaryAccountId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Beneficiaries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Beneficiaries_Accounts_BeneficiaryAccountId",
                        column: x => x.BeneficiaryAccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Beneficiaries_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Beneficiaries_BeneficiaryAccountId",
                table: "Beneficiaries",
                column: "BeneficiaryAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Beneficiaries_UserId",
                table: "Beneficiaries",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Beneficiaries");
        }
    }
}
