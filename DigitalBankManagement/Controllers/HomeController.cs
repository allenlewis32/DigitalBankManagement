using System.Diagnostics;
using DigitalBankManagement.Data;
using DigitalBankManagement.Models;
using Microsoft.AspNetCore.Mvc;

namespace DigitalBankManagement.Controllers
{
	public class HomeController : Controller
	{
		private readonly ApplicationDbContext _context;

		public HomeController(ApplicationDbContext context)
		{
			_context = context;
		}

		public IActionResult Index()
		{
			var sessionId = Request.Cookies["sessionId"];
			if (sessionId != null)
			{
				var user = Helper.GetUser(_context, sessionId);
				if (user != null)
				{
					string controller = "";
					switch (user.Role.Name)
					{
						case "admin":
							controller = "Admin";
							break;
						case "manager":
							controller = "Manager";
							break;
						case "user":
							controller = "User";
							break;
					}
					return RedirectToAction("Index", controller);
				}
				else
				{
					foreach (var cookie in Request.Cookies.Keys)
					{
						Response.Cookies.Delete(cookie);
					}
				}
			}
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}