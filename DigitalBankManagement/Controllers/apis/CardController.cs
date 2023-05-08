using DigitalBankManagement.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DigitalBankManagement.Controllers.apis
{
	[Route("api/[controller]")]
	[ApiController]
	public class CardController : ControllerBase
	{
		private readonly ApplicationDbContext _context;
		public CardController(ApplicationDbContext context)
		{
			_context = context;
		}

		[HttpGet]
		public IActionResult Get([FromHeader] string sessionId)
		{
			try
			{
				var user = Helper.GetUser(_context, sessionId);
				if (user == null)
				{
					return Unauthorized();
				}

				var cards = _context.Cards.Include(card => card.Account)
					.Where(card => card.Account!.UserId == user.Id && card.Expiry > DateTime.UtcNow);
				return Ok(cards);
			}
			catch
			{
				return Problem();
			}
		}

		public class CardAddModel
		{
			public int AccountId { get; set; }
			public int Pin { get; set; }
		}

		[HttpPost]
		public IActionResult Post([FromHeader] string sessionId, CardAddModel model)
		{
			try
			{
				var user = Helper.GetUser(_context, sessionId);
				if (user == null)
				{
					return Unauthorized();
				}

				var account = _context.Accounts.FirstOrDefault(acc => acc.Id == model.AccountId);
				if (account == null)
				{
					return BadRequest();
				}
				if (account.UserId != user.Id)
				{
					return Unauthorized();
				}

				decimal cardNumber;
				if (_context.Cards.Any())
				{
					cardNumber = _context.Cards.Max(card => card.Id) + 1;
				}
				else
				{
					cardNumber = 4000000000000000; // initial card number
				}
				_context.Cards.Add(new()
				{
					AccountId = model.AccountId,
					Expiry = DateTime.UtcNow.AddYears(5),
					Pin = model.Pin,
					Id = cardNumber,
				});
				_context.SaveChanges();

				return Ok();
			}
			catch
			{
				return Problem();
			}
		}

		[HttpDelete]
		public IActionResult Delete([FromHeader] string sessionId, string cardNumber)
		{
			try
			{
				var user = Helper.GetUser(_context, sessionId);
				if (user == null)
				{
					return Unauthorized();
				}

				decimal cardId = decimal.Parse(cardNumber);
				var card = _context.Cards.FirstOrDefault(card => card.Id == cardId);
				if (card == null)
				{
					return BadRequest();
				}

				var account = _context.Accounts.FirstOrDefault(acc => acc.Id == card.AccountId);
				if (account == null)
				{
					return BadRequest();
				}
				if (account.UserId != user.Id)
				{
					return Unauthorized();
				}

				card.Expiry = DateTime.UtcNow.AddSeconds(-1);
				_context.SaveChanges();
				return Ok();
			}
			catch
			{
				return Problem();
			}
		}
	}
}
