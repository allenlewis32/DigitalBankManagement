using System.Net.NetworkInformation;
using DigitalBankManagement.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DigitalBankManagement.Controllers.apis
{
	[Route("api/[controller]")]
	[ApiController]
	public class ATMController : ControllerBase
	{
		private readonly ApplicationDbContext _context;
		public ATMController(ApplicationDbContext context)
		{
			_context = context;
		}

		[HttpPost]
		[Route("Credit")]
		public IActionResult Credit([FromForm] string cardNumber, [FromForm] decimal amount, [FromForm] int pin)
		{
			try
			{
				// verify card
				decimal cardId = decimal.Parse(cardNumber);
				var card = _context.Cards.Include(card => card.Account).FirstOrDefault(card => card.Id == cardId && card.Expiry > DateTime.UtcNow);
				if (card == null)
				{
					return BadRequest();
				}
				if(card.Pin != pin)
				{
					return Unauthorized();
				}

				return Helper.TransferMoney(_context, this, null, amount, card.Account);
			}
			catch
			{
				return Problem();
			}
		}

		[HttpPost]
		[Route("Debit")]
		public IActionResult Debit([FromForm] string cardNumber, [FromForm] decimal amount, [FromForm] int pin)
		{
			try
			{
				// verify card
				decimal cardId = decimal.Parse(cardNumber);
				var card = _context.Cards.Include(card => card.Account).FirstOrDefault(card => card.Id == cardId && card.Expiry > DateTime.UtcNow);
				if (card == null)
				{
					return BadRequest();
				}
				if (card.Pin != pin)
				{
					return Unauthorized();
				}

				return Helper.TransferMoney(_context, this, card.Account, amount, null);
			}
			catch
			{
				return Problem();
			}
		}
	}
}
