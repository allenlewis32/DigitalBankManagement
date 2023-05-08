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

		public class Model
		{
			public string CardNumber { get; set; }
			public decimal Amount { get; set; }
			public int Pin { get; set; }
		}

		[HttpPost]
		[Route("Credit")]
		public IActionResult Credit(Model model)
		{
			try
			{
				// verify card
				decimal cardId = decimal.Parse(model.CardNumber);
				var card = _context.Cards.Include(card => card.Account).FirstOrDefault(card => card.Id == cardId && card.Expiry > DateTime.UtcNow);
				if (card == null)
				{
					return BadRequest();
				}
				if(card.Pin != model.Pin)
				{
					return Unauthorized();
				}

				return Helper.TransferMoney(_context, this, null, model.Amount, card.Account);
			}
			catch
			{
				return Problem();
			}
		}

		[HttpPost]
		[Route("Debit")]
		public IActionResult Debit(Model model)
		{
			try
			{
				// verify card
				decimal cardId = decimal.Parse(model.CardNumber);
				var card = _context.Cards.Include(card => card.Account).FirstOrDefault(card => card.Id == cardId && card.Expiry > DateTime.UtcNow);
				if (card == null)
				{
					return BadRequest();
				}
				if (card.Pin != model.Pin)
				{
					return Unauthorized();
				}

				return Helper.TransferMoney(_context, this, card.Account, model.Amount, null);
			}
			catch
			{
				return Problem();
			}
		}
	}
}
