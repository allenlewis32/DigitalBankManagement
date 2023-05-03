using DigitalBankManagement.Data;
using DigitalBankManagement.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DigitalBankManagement.Controllers.apis
{
	[Route("api/[controller]")]
	[ApiController]
	public class TransactionController : ControllerBase
	{
		private readonly ApplicationDbContext _context;
		public TransactionController(ApplicationDbContext context)
		{
			_context = context;
		}

		// performs transactions
		[HttpPost]
		public IActionResult Post([FromHeader] string sessionId, [FromForm] int accountId, [FromForm] int beneficiaryId, [FromForm] int amount)
		{
			try
			{
				var user = Helper.GetUser(_context, sessionId);
				if (user == null)
				{
					return Unauthorized();
				}

				// verify accountId belongs to user
				var account = _context.Accounts.FirstOrDefault(acc => acc.Id == accountId);
				if (account == null)
				{
					return BadRequest();
				}
				if (account.UserId != user.Id)
				{
					return Unauthorized();
				}

				var beneficiary = _context.Beneficiaries.Include(b => b.BeneficiaryAccount)
					.FirstOrDefault(b => b.Id == beneficiaryId);
				if(beneficiary == null)
				{
					return BadRequest();
				}
				var beneficiaryAccount = beneficiary.BeneficiaryAccount;
				return Helper.TransferMoney(_context, this, account, amount, beneficiaryAccount);
			}
			catch(Exception ex)
			{
				return Problem(ex.ToString());
			}
		}
	}
}
