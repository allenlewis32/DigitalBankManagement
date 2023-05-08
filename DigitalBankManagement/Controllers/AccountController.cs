using DigitalBankManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace DigitalBankManagement.Controllers
{
	public class AccountController : Controller
	{
		[HttpPost]
		[Route("/Account/CreateAccount", Name = "CreateAccount")]
		public IActionResult CreateAccount(CreateAccountModel model)
		{
			try
			{
				dynamic? res = Helper.Post(this, "Account", "CreateAccount", Request.Cookies["sessionId"], TempData, model);
				return RedirectToRoute("UserIndex");
			}
			catch // unauthorized
			{
				return RedirectToRoute("Login");
			}
		}

		[Route("/Account/GetStatement", Name = "GetStatement")]
		public IActionResult GetStatement(string accountId)
		{
			try
			{
				var query = new Dictionary<string, string>
				{
					["accountId"] = accountId
				};
				dynamic? res = Helper.Get(this, "Account", "GetStatement", Request.Cookies["sessionId"], TempData, query);
				foreach (var transaction in (JArray)res!)
				{
					var creditOrDebit = "";
					var creditOrDebitClass = "";
					var remarks = "";
					if ((bool)transaction["credit"])
					{
						creditOrDebit = "Credit";
						creditOrDebitClass = "text-success";
						remarks = "Transfered from ";
					}
					else
					{
						creditOrDebit = "Debit";
						creditOrDebitClass = "text-danger";
						remarks = "Transfered to ";
					}
					if (transaction["otherAccountId"]!.ToString() == "")
					{
						remarks = "ATM transaction";
					}
					else
					{
						remarks += "account: " + transaction["otherAccountId"];
					}
					transaction["creditOrDebit"] = creditOrDebit;
					transaction["creditOrDebitClass"] = creditOrDebitClass;
					transaction["remarks"] = remarks;
				}
				ViewData["statement"] = res;
				return View();
			}
			catch // unauthorized
			{
				return RedirectToRoute("Login");
			}
		}
	}
}
