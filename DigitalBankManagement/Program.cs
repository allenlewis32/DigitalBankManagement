using DigitalBankManagement.Data;
using DigitalBankManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DigitalBankManagement
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			builder.Services.AddControllers();

			var connectionString = builder.Configuration.GetConnectionString("default") ?? throw new Exception("Connection string not found");

			// Add services to the container.
			builder.Services.AddDbContext<ApplicationDbContext>(options =>
				options.UseSqlServer(connectionString));
			builder.Services.AddDatabaseDeveloperPageExceptionFilter();

			builder.Services.AddControllersWithViews();

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();

			using (var scope = app.Services.CreateScope())
			{
				IServiceProvider serviceProvider = scope.ServiceProvider;
				EnsureRolesExist(serviceProvider.GetRequiredService<ApplicationDbContext>());
				EnsureAdminExists(serviceProvider.GetRequiredService<ApplicationDbContext>());
			}

			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");

			app.MapControllers();

			app.Run();
		}

		// Checks whether the necessary roles exist and creates them if they don't exist
		private static void EnsureRolesExist(ApplicationDbContext _context)
		{
			var roles = new string[] { "admin", "manager", "user" };
			foreach (var role in roles)
			{
				if (_context.Roles.FirstOrDefault(r => r.Name == role) == null)
				{
					_context.Roles.Add(new() { Name = role });
				}
			}
			_context.SaveChanges();
		}

		// checks whether admin exists and creates an admin account if it doesn't exist
		private static void EnsureAdminExists(ApplicationDbContext _context)
		{
			if (_context.Users.FirstOrDefault(u => u.UserName == "admin") == null)
			{
				var adminUser = new UserModel()
				{
					UserName = "admin",
					Email = "",
					RoleId = _context.Roles.First(r => r.Name == "admin").Id,
					FirstName = "Admin",
					LastName = "",
					Active = true
				};
				adminUser.PasswordHash = new PasswordHasher<UserModel>().HashPassword(adminUser, "Admin@123");
				_context.Users.Add(adminUser);
			}
			_context.SaveChanges();
		}
	}
}