using DigitalBankManagement.Models;
using Microsoft.AspNetCore.Mvc;

namespace DigitalBankManagement.Controllers
{
	public class AuthenticationController : Controller
	{
		[HttpGet]
		[Route("/Authentication/Login", Name = "Login")]
		public IActionResult Login()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Login(LoginModel model)
		{
			if (ModelState.IsValid)
			{
				dynamic? json = Helper.Post(this, "Authentication", "Login", null, TempData, model);
				if (json != null)
				{
					return SetCookieAndRedirect(json);
				}
			}
			return View();
		}

		[HttpGet]
		[Route("/Authentication/Register", Name = "Register")]
		public IActionResult Register()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Register(RegisterModel model)
		{
			if (ModelState.IsValid)
			{
				dynamic? json = Helper.Post(this, "Authentication", "Register", null, TempData, model);
				if (json != null)
				{
					return SetCookieAndRedirect(json);
				}
			}
			return View();
		}

		// sets cookies and redirects to the appropriate home page depending upon the role of the user
		private IActionResult SetCookieAndRedirect(dynamic json)
		{
			Response.Cookies.Append("sessionId", (string)json.sessionId, new CookieOptions()
			{
				MaxAge = TimeSpan.FromDays(7),
			});

			Response.Cookies.Append("role", (string)json.role, new CookieOptions()
			{
				MaxAge = TimeSpan.FromDays(7)
			});

			Response.Cookies.Append("firstName", (string)json.firstName, new CookieOptions()
			{
				MaxAge = TimeSpan.FromDays(7)
			});

			Response.Cookies.Append("lastName", (string)json.lastName, new CookieOptions()
			{
				MaxAge = TimeSpan.FromDays(7)
			});

			string controller = "";
			switch ((string)json.role)
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

		[HttpGet]
		[Route("/Authentication/Logout", Name = "Logout")]
		public IActionResult Logout()
		{
			Helper.Get(this, "Authentication", "Logout", Request.Cookies["sessionId"]!, TempData);
			Response.Cookies.Delete("sessionId");
			return RedirectToAction("Index", "Home");
		}
	}
}
