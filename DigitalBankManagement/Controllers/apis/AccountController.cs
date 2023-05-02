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
				var res = new Dictionary<string, string>();
				switch (user.Role.Name)
				{
					case "admin":
						res["savings"] = _context.Accounts
							.Count(account => account.Type == AccountModel.TypeSavings).ToString();
						res["fd"] = _context.Accounts
							.Count(account => account.Type == AccountModel.TypeFD).ToString();
						res["rd"] = _context.Accounts
							.Count(account => account.Type == AccountModel.TypeRD).ToString();
						res["loan"] = _context.Accounts
							.Count(account => account.Type == AccountModel.TypeLoan).ToString();
						res["managers"] = _context.Users
							.Count(user =>
							user.RoleId == _context.Roles.First(role => role.Name == "manager").Id)
							.ToString();
						res["users"] = _context.Users
							.Count(user =>
							user.RoleId == _context.Roles.First(role => role.Name == "user").Id)
							.ToString();
						break;
					case "user":
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
