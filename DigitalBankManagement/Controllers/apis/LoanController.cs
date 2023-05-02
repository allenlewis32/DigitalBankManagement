using DigitalBankManagement.Data;
using DigitalBankManagement.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DigitalBankManagement.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class LoanController : ControllerBase
	{
		private readonly ApplicationDbContext _context;
		public LoanController(ApplicationDbContext context)
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
				IQueryable<LoanModel> loans = _context.Loans.Include(loan => loan.Account)
					.Where(loan => loan.Account.Active);
				if (user.Role.Name.ToLower() != "admin")
				{
					loans = loans.Where(loan => loan.Account.UserId == user.Id);
				}

				// don't send user information
				loans.ForEachAsync(loan =>
				{
					loan.Account.User = null;
				}).Wait();
				return Ok(loans);
			}
			catch
			{
				return Problem();
			}
		}
	}
}
