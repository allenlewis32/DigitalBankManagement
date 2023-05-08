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

		public class Model
		{
			public int AccountId { get; set; }
			public int BeneficiaryId { get; set; }
			public int Amount { get; set; }
		}

		// performs transactions
		[HttpPost]
		public IActionResult Post([FromHeader] string sessionId, Model model)
		{
			try
			{
				var user = Helper.GetUser(_context, sessionId);
				if (user == null)
				{
					return Unauthorized();
				}

				// verify accountId belongs to user
				var account = _context.Accounts.FirstOrDefault(acc => acc.Id == model.AccountId);
				if (account == null)
				{
					return BadRequest();
				}
				if (account.UserId != user.Id)
				{
					return Unauthorized();
				}

				var beneficiary = _context.Beneficiaries.Include(b => b.BeneficiaryAccount)
					.FirstOrDefault(b => b.Id == model.BeneficiaryId);
				if(beneficiary == null)
				{
					return BadRequest();
				}
				var beneficiaryAccount = beneficiary.BeneficiaryAccount;
				return Helper.TransferMoney(_context, this, account, model.Amount, beneficiaryAccount);
			}
			catch(Exception ex)
			{
				return Problem(ex.ToString());
			}
		}
	}
}
