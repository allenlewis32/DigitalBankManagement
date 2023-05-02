using System.Security.Principal;
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
						res["fd"] = _context.FdAccounts.Include(fd => fd.Account).ToList();
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

		// Creates a savings, fd or rd account. To create a loan account, use the ApplyLoan method
		[HttpPost]
		[Route("CreateAccount")]
		public IActionResult CreateAccount([FromHeader] string sessionId, [FromForm] CreateAccountModel createAccountModel)
		{
			try
			{
				var user = Helper.GetUser(_context, sessionId);
				if (user == null)
				{
					return Unauthorized();
				}
				var newAccount = new AccountModel()
				{
					Active = true,
					DateCreated = DateTime.UtcNow,
					Type = createAccountModel.Type,
					UserId = user.Id,
				};
				switch (createAccountModel.Type)
				{
					case AccountModel.TypeSavings:
						_context.Accounts.Add(newAccount);
						break;
					case AccountModel.TypeFD:
						// create FD account
						_context.FdAccounts.Add(new()
						{
							Account = newAccount,
							InitialDeposit = (decimal)createAccountModel.InitialDeposit!,
							Duration = (int)createAccountModel.Duration!,
						});

						// perform transaction
						return TransferMoney((int)createAccountModel.DebitFrom!, (decimal)createAccountModel.InitialDeposit!, newAccount);
					case AccountModel.TypeRD:
						// create RD account
						_context.RdAccounts.Add(new()
						{
							Account = newAccount,
							DebitFrom = createAccountModel.DebitFrom,
							Duration = (int)createAccountModel.Duration!,
							MonthlyDeposit = (decimal)createAccountModel.MonthlyDeposit!
						});

						// store transaction
						_context.Transactions.Add(new()
						{
							FromAccountId = createAccountModel.DebitFrom,
							ToAccount = newAccount,
							Time = DateTime.UtcNow
						});

						// perform transaction
						return TransferMoney((int)createAccountModel.DebitFrom!, (decimal)createAccountModel.MonthlyDeposit!, newAccount);
					default: // cannot create loan or invalid type
						return BadRequest();
				}
				_context.SaveChanges();
				return Ok();
			}
			catch
			{
				return Problem();
			}
		}
		private IActionResult TransferMoney(int debitFrom, decimal amount, AccountModel creditToAccount)
		{
			var debitFromAccount = _context.Accounts
				.First(account => account.Id == debitFrom);

			// verify that the debit account is a savings account
			if (debitFromAccount.Type != AccountModel.TypeSavings)
			{
				return BadRequest("Money can be debited only from savings account");
			}

			// verify balance
			if (debitFromAccount.Amount < amount)
			{
				return Conflict("Not enough amount");
			}

			debitFromAccount.Amount -= amount;
			creditToAccount.Amount = amount; // don't need to add as it is a new account

			// store transaction
			_context.Transactions.Add(new()
			{
				FromAccount = debitFromAccount,
				ToAccount = creditToAccount,
				Time = DateTime.UtcNow
			});
			// Finally, save changes
			_context.SaveChanges();
			return Ok();
		}

		[HttpPost]
		[Route("ApplyLoan")]
		public IActionResult ApplyLoan([FromHeader] string sessionId, [FromForm] decimal amount, [FromForm] int duration, [FromForm] int debitFrom)
		{
			try
			{
				var user = Helper.GetUser(_context, sessionId);
				if (user == null)
				{
					return Unauthorized();
				}

				var debitFromAccount = _context.Accounts
					.First(account => account.Id == debitFrom);

				// verify that the debit account is a savings account
				if (debitFromAccount.Type != AccountModel.TypeSavings)
				{
					return BadRequest("Money can be debited only from savings account");
				}

				var loanApplication = new LoanApplicationModel()
				{
					Amount = amount,
					DebitFrom = debitFrom,
					Duration = duration,
					Status = 0,
					User = user
				};

				_context.LoanApplications.Add(loanApplication);
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
