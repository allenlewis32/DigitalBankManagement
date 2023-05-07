using System.Text;
using DigitalBankManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Protocol;

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
				dynamic? json = await Helper.PostAsync(this, "Authentication", "Login", null, ViewData, model);
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
				dynamic? json = await Helper.PostAsync(this, "Authentication", "Register", null, ViewData, model);
				if (json != null)
				{
					return SetCookieAndRedirect(json);
				}
			}
			return View();
		}

		// sets sessionId and redirects to the appropriate home page depending upon the role of the user
		private IActionResult SetCookieAndRedirect(dynamic json)
		{
			Response.Cookies.Append("sessionId", (string)json.sessionId, new CookieOptions()
			{
				MaxAge = TimeSpan.FromDays(7),
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
			Helper.PostAsync(this, "Authentication", "Logout", Request.Cookies["sessionId"]!, ViewData).Wait();
			Response.Cookies.Delete("sessionId");
			return RedirectToAction("Index", "Home");
		}
	}
}
