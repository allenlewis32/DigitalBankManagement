using DigitalBankManagement.Data;
using DigitalBankManagement.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DigitalBankManagement.Controllers.apis
{
	[Route("api/[controller]")]
	[ApiController]
	public class BeneficiaryController : ControllerBase
	{
		private readonly ApplicationDbContext _context;
		public BeneficiaryController(ApplicationDbContext context)
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

				var beneficiaries = new List<BeneficiaryModel>();
				_context.Beneficiaries.Where(b => b.UserId == user.Id)
					.ForEachAsync(beneficiary =>
					{
						beneficiaries.Add(new()
						{
							Id = beneficiary.Id,
							UserId = beneficiary.UserId,
							Name = beneficiary.Name,
							BeneficiaryAccountId = beneficiary.BeneficiaryAccountId
						});
					})
					.Wait();
				return Ok(beneficiaries);
			}
			catch
			{
				return Problem();
			}
		}

		[HttpPost]
		public IActionResult Post([FromHeader] string sessionId, [FromForm] string name, [FromForm] int accountId)
		{
			try
			{
				var user = Helper.GetUser(_context, sessionId);
				if (user == null)
				{
					return Unauthorized();
				}
				
				// verify beneficiary account
				var account = _context.Accounts.First(acc => acc.Id == accountId);
				if (account == null || !account.Active || account.Type != AccountModel.TypeSavings)
				{
					return Conflict("Invalid account details");
				}
				
				_context.Beneficiaries.Add(new()
				{
					BeneficiaryAccountId = accountId,
					Name = name,
					User = user,
				});
				_context.SaveChanges();
				return Ok();
			}
			catch
			{
				return Problem();
			}
		}

		[HttpPut]
		public IActionResult Put([FromHeader] string sessionId, [FromForm] int beneficiaryId, [FromForm] string name, [FromForm] int accountId)
		{
			try
			{
				var user = Helper.GetUser(_context, sessionId);
				if (user == null)
				{
					return Unauthorized();
				}

				// load beneficiary
				var beneficiary = _context.Beneficiaries.FirstOrDefault(b => b.Id == beneficiaryId);
				// verify beneficiary
				if (beneficiary == null)
				{
					return BadRequest();
				}
				// verify whether the beneficiary belongs to the user
				if (beneficiary.UserId != user.Id)
				{
					return Unauthorized();
				}

				// verify beneficiary account
				var account = _context.Accounts.First(acc => acc.Id == accountId);
				if (account == null || !account.Active || account.Type != AccountModel.TypeSavings)
				{
					return Conflict("Invalid account details");
				}

				beneficiary.Name = name;
				beneficiary.BeneficiaryAccountId = accountId;
				_context.SaveChanges();
				return Ok();
			}
			catch
			{
				return Problem();
			}
		}

		[HttpDelete]
		public IActionResult Delete([FromHeader] string sessionId, [FromForm] int beneficiaryId)
		{
			try
			{
				var user = Helper.GetUser(_context, sessionId);
				if (user == null)
				{
					return Unauthorized();
				}

				// load beneficiary
				var beneficiary = _context.Beneficiaries.FirstOrDefault(b => b.Id == beneficiaryId);
				// verify beneficiary
				if (beneficiary == null)
				{
					return BadRequest();
				}
				// verify whether the beneficiary belongs to the user
				if (beneficiary.UserId != user.Id)
				{
					return Unauthorized();
				}

				_context.Beneficiaries.Remove(beneficiary);
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
