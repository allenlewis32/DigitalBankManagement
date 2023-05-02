using DigitalBankManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace DigitalBankManagement.Data
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

		public DbSet<UserModel> Users { get; set; }
		public DbSet<RoleModel> Roles { get; set; }
		public DbSet<SessionModel> Sessions { get; set; }
		public DbSet<AccountModel> Accounts { get; set; }
		public DbSet<FdAccountModel> FdAccounts { get; set; }
		public DbSet<RdAccountModel> RdAccounts { get; set; }
		public DbSet<LoanModel> Loans { get; set; }
		public DbSet<LoanApplicationModel> LoanApplications { get; set; }
		public DbSet<InterestModel> Interests { get; set; }
		public DbSet<TransactionModel> Transactions { get; set; }
	}
}
