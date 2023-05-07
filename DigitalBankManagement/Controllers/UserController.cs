using Microsoft.AspNetCore.Mvc;

namespace DigitalBankManagement.Controllers
{
	public class UserController : Controller
	{
		[Route("/User/Index", Name = "UserIndex")]
		public IActionResult Index()
		{
			try
			{
				dynamic? accounts = Helper.Get(this, "Account", null, Request.Cookies["sessionId"], TempData);
				ViewData["accounts"] = accounts; // use ViewData so that it can be accessed later by CreateAccountModal
				return View();
			}
			catch // unauthorized
			{
				return RedirectToRoute("Login");
			}
		}
	}
}
