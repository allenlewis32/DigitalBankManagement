﻿using DigitalBankManagement.Data;
using DigitalBankManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DigitalBankManagement.Controllers.apis
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
							.Count(account => account.Type == AccountModel.TypeSavings && account.Active);
						res["fd"] = _context.Accounts
							.Count(account => account.Type == AccountModel.TypeFD && account.Active);
						res["rd"] = _context.Accounts
							.Count(account => account.Type == AccountModel.TypeRD && account.Active);
						res["loan"] = _context.Accounts
							.Count(account => account.Type == AccountModel.TypeLoan && account.Active);
						res["managers"] = _context.Users
							.Count(user =>
							user.RoleId == _context.Roles.First(role => role.Name == "manager").Id);
						res["users"] = _context.Users
							.Count(user =>
							user.RoleId == _context.Roles.First(role => role.Name == "user").Id);
						break;
					case "user":
						res["savings"] = _context.Accounts.Where(account => account.UserId == user.Id && account.Type == AccountModel.TypeSavings && account.Active).ToList();
						res["fd"] = _context.FdAccounts.Include(fd => fd.Account).Where(acc => acc.Account.UserId == user.Id && acc.Account.Active).ToList();
						res["rd"] = _context.RdAccounts.Include(rd => rd.Account).Where(acc => acc.Account.UserId == user.Id && acc.Account.Active).ToList();
						res["loan"] = _context.Loans.Include(loan => loan.Account).Where(acc => acc.Account.UserId == user.Id && acc.Account.Active).ToList();
						break;
				}
				return Ok(res);
			}
			catch (Exception ex)
			{
				return Problem(ex.Message);
			}
		}

		// Creates a savings, fd or rd account. To create a loan account, use the ApplyLoan method
		[HttpPost]
		[Route("CreateAccount")]
		public IActionResult CreateAccount([FromHeader] string sessionId, CreateAccountModel createAccountModel)
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
						var debitFromAccount = _context.Accounts
							.First(acc => acc.Id == (int)createAccountModel.DebitFrom!);
						return Helper.TransferMoney(_context, this, debitFromAccount, (decimal)createAccountModel.InitialDeposit!, newAccount);
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
						debitFromAccount = _context.Accounts
							.First(acc => acc.Id == (int)createAccountModel.DebitFrom!);
						return Helper.TransferMoney(_context, this, debitFromAccount, (decimal)createAccountModel.MonthlyDeposit!, newAccount);
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

		public class ApplyLoanModel
		{
			public decimal Amount { get; set; }
			public int Duration { get; set; }
			public int DebitFrom { get; set; }
		}

		[HttpPost]
		[Route("ApplyLoan")]
		public IActionResult ApplyLoan([FromHeader] string sessionId, ApplyLoanModel model)
		{
			try
			{
				var user = Helper.GetUser(_context, sessionId);
				if (user == null)
				{
					return Unauthorized();
				}

				var debitFromAccount = _context.Accounts
					.First(account => account.Id == model.DebitFrom);

				// verify that the debit account is a savings account
				if (debitFromAccount.Type != AccountModel.TypeSavings)
				{
					return BadRequest("Money can be debited only from savings account");
				}

				var loanApplication = new LoanApplicationModel()
				{
					Amount = model.Amount,
					DebitFrom = model.DebitFrom,
					Duration = model.Duration,
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

		[HttpGet]
		[Route("Deactivate")]
		public IActionResult Deactivate([FromHeader] string sessionId, int accountId, int? transferTo)
		{
			try
			{
				var user = Helper.GetUser(_context, sessionId);
				if (user == null)
				{
					return Unauthorized();
				}
				var account = _context.Accounts.First(account => account.Id == accountId);
				if (account.UserId != user.Id)
				{
					return Unauthorized();
				}
				if (account.Type == AccountModel.TypeLoan)
				{
					return BadRequest("Can't close loan account");
				}
				bool transferMoney = false;
				AccountModel? transferToAccount = null;
				if (transferTo != null)
				{
					transferToAccount = _context.Accounts.First(account => account.Id == transferTo);
					if (transferToAccount.Type != AccountModel.TypeSavings)
					{
						return BadRequest("Money can be transfered only to a savings account");
					}
				}
				if (account.Amount != 0)
				{
					if (transferToAccount == null)
					{
						return BadRequest("Cannot close non-empty account. Transfer money to another account or extract it from the ATM");
					}
					transferMoney = true;
				}
				account.Active = false;
				if (transferMoney)
				{
					return Helper.TransferMoney(_context, this, account, account.Amount, _context.Accounts.First(account => account.Id == transferTo), true); // saveChanges will be called in TransferMoney
				}
				return Ok();
			}
			catch
			{
				return Problem();
			}
		}

		[HttpGet]
		[Route("GetStatement")]
		public IActionResult GetStatement([FromHeader] string sessionId, string accountId)
		{
			try
			{
				var user = Helper.GetUser(_context, sessionId);
				if (user == null)
				{
					return Unauthorized();
				}
				int accountIdInt = int.Parse(accountId);
				var account = _context.Accounts.First(account => account.Id == accountIdInt);
				if (account.UserId != user.Id)
				{
					return Unauthorized();
				}
				var statements = new List<dynamic>();
				_context.Transactions
					.Where(transaction => transaction.FromAccountId == accountIdInt || transaction.ToAccountId == accountIdInt)
					.ForEachAsync(transaction =>
					{
						var credit = transaction.ToAccountId == accountIdInt;
						int? otherAccountId = null;
						if (credit)
						{
							otherAccountId = transaction.FromAccountId;
						} else
						{
							otherAccountId = transaction.ToAccountId;
						}
						// create new objects to prevent account information from leaking
						statements.Add(new
						{
							transaction.Id,
							credit,
							otherAccountId,
							transaction.Time,
							transaction.Amount
						});
					})
					.Wait();
				return Ok(statements);
			}
			catch (Exception ex)
			{
				return Problem(ex.Message);
			}
		}
	}
}
