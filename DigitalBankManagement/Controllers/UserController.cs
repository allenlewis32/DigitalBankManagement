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
				TempData["savings"] = accounts;
				return View();
			} catch (Exception ex)
			{
				TempData["errorMessage"] = ex.Message;
				return RedirectToRoute("Login");
			}
		}
	}
}
