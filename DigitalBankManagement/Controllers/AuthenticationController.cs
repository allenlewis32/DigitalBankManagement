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

        [HttpGet]
        [Route("/Authentication/Register", Name = "Register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if(ModelState.IsValid)
            {
				using var client = new HttpClient();
				string baseUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}/api/Authentication/"; // gets the base URL of this website
				client.BaseAddress = new Uri(baseUrl);
				/*HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(model));
				var responseTask = client.PostAsync("Register", httpContent); // don't use await as we have to check result
				responseTask.Wait();*/
				var responseTask = client.PostAsJsonAsync("Register", model);
				responseTask.Wait();

				var result = responseTask.Result;
				if (result.IsSuccessStatusCode)
				{
					var content = await result.Content.ReadAsStringAsync();
					dynamic json = JsonConvert.DeserializeObject(content);

					// set cookies
					Response.Cookies.Append("sessionId", (string)json.sessionId, new CookieOptions()
					{
						MaxAge = TimeSpan.FromDays(7),
					});

					string controller = "";
					switch ((string) json.role)
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
					ViewData["errorMessage"] = await result.Content.ReadAsStringAsync();
				}
			}
            return View();
        }
    }
}
