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
	}
}
