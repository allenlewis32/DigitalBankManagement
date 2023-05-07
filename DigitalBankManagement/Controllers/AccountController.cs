using DigitalBankManagement.Models;
using Microsoft.AspNetCore.Mvc;

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
	}
}
