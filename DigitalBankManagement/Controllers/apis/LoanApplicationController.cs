using System.ComponentModel.DataAnnotations;
using DigitalBankManagement.Data;
using DigitalBankManagement.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DigitalBankManagement.Controllers.apis
{
	[Route("api/[controller]")]
	[ApiController]
	public class LoanApplicationController : ControllerBase
	{
		private readonly ApplicationDbContext _context;
		public LoanApplicationController(ApplicationDbContext context)
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
				IQueryable<LoanApplicationModel> applications = _context.LoanApplications;
				if (user.Role.Name.ToLower() == "user")
				{
					applications = applications.Where(application => application.UserId == user.Id);
				}
				var result = new List<LoanApplicationModel>();
				applications.ForEachAsync(application =>
				{
					// create a new object to prevent sensitive information from leaking
					result.Add(new()
					{
						Id = application.Id,
						UserId = application.UserId,
						Amount = application.Amount,
						DebitFrom = application.DebitFrom,
						Status = application.Status,
						Duration = application.Duration
					});
				}).Wait();
				return Ok(result);
			}
			catch
			{
				return Problem();
			}
		}

		public class Model
		{
			public int ApplicationId { get; set;}
		}

		[HttpPost]
		[Route("Reject")]
		public IActionResult Reject([FromHeader] string sessionId, Model model)
		{
			try
			{
				UserModel? user = Helper.GetUser(_context, sessionId);
				if (user == null || user.Role.Name == "user")
				{
					return Unauthorized();
				}
				LoanApplicationModel application = _context.LoanApplications.First(la => la.Id == model.ApplicationId);
				application.Status = -1;
				_context.SaveChanges();
				return Ok();
			}
			catch
			{
				return Problem();
			}
		}

		[HttpPost]
		[Route("Approve")]
		public IActionResult Approve([FromHeader] string sessionId, Model model)
		{
			try
			{
				UserModel? user = Helper.GetUser(_context, sessionId);
				if (user == null || user.Role.Name == "user")
				{
					return Unauthorized();
				}
				LoanApplicationModel application = _context.LoanApplications.First(la => la.Id == model.ApplicationId);
				application.Status = 1;
				AccountModel account = new()
				{
					UserId = application.UserId,
					Amount = application.Amount,
					DateCreated = DateTime.UtcNow,
					Active = true,
					Type = AccountModel.TypeLoan,
				};
				LoanModel loan = new()
				{
					Account = account,
					Duration = application.Duration,
					DebitFrom = application.DebitFrom,
				};
				CalculateEmi(loan);
				_context.Loans.Add(loan);
				_context.SaveChanges();
				return Ok();
			}
			catch(Exception ex)
			{
				return Problem(ex.Message);
			}
		}

		// calculates Emi for loan
		private void CalculateEmi(LoanModel loan)
		{
			decimal P = loan.Account.Amount;
			decimal R = _context.Interests.First().Loan;
			int N = loan.Duration;
			decimal t = (decimal)Math.Pow((double)(1 + R / 1200), N);
			decimal emi = P * R * t / (t - 1) / (N * 12);
			loan.Emi = emi;
		}
	}
}
