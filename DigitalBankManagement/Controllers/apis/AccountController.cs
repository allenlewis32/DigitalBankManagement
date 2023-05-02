using DigitalBankManagement.Data;
using DigitalBankManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DigitalBankManagement.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AccountController : ControllerBase
	{
		private readonly ApplicationDbContext _context;
		public AccountController(ApplicationDbContext context)
		{
			_context = context;
		}
		[HttpGet]
		public IActionResult Get([FromHeader] string sessionId)
		{
			try
			{
				UserModel? user = Helper.GetUser(_context, sessionId);
				if (user == null)
				{
					return Unauthorized();
				}
				var res = new Dictionary<string, object>();
				switch (user.Role.Name)
				{
					case "admin":
						res["savings"] = _context.Accounts
							.Count(account => account.Type == AccountModel.TypeSavings);
						res["fd"] = _context.Accounts
							.Count(account => account.Type == AccountModel.TypeFD);
						res["rd"] = _context.Accounts
							.Count(account => account.Type == AccountModel.TypeRD);
						res["loan"] = _context.Accounts
							.Count(account => account.Type == AccountModel.TypeLoan);
						res["managers"] = _context.Users
							.Count(user =>
							user.RoleId == _context.Roles.First(role => role.Name == "manager").Id);
						res["users"] = _context.Users
							.Count(user =>
							user.RoleId == _context.Roles.First(role => role.Name == "user").Id);
						break;
					case "user":
						res["savings"] = _context.Accounts.Where(account => account.Type == AccountModel.TypeSavings).ToList();
						res["fd"] = _context.Accounts.Where(account => account.Type == AccountModel.TypeFD).ToList();
						res["rd"] = _context.RdAccounts.Include(rd => rd.Account).ToList();
						res["loan"] = _context.Loans.Include(loan => loan.Account).ToList();
						break;
				}
				return Ok(res);
			}
			catch
			{
				return Problem();
			}
		}
	}
}
