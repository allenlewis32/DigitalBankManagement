using Microsoft.AspNetCore.Mvc;

namespace DigitalBankManagement.Controllers
{
	public class CardController : Controller
	{
		[Route("/Card/Index", Name = "CardIndex")]
		public IActionResult Index()
		{
			try
			{
				dynamic? cards = Helper.Get(this, "Card", null, Request.Cookies["sessionId"], TempData);
				ViewData["cards"] = cards;
				return View();
			}
			catch // unauthorized
			{
				return RedirectToRoute("Login");
			}
		}

		[HttpPost]
		[Route("/Card/Add", Name = "CardAdd")]
		public IActionResult CardAdd(string accountId, string pin)
		{
			try
			{
				var parameters = new
				{
					accountId,
					pin,
				};
				dynamic? res = Helper.Post(this, "Card", null, Request.Cookies["sessionId"], TempData, parameters);
				return RedirectToRoute("CardIndex");
			}
			catch // unauthorized
			{
				return RedirectToRoute("Login");
			}
		}

		[HttpGet]
		[Route("/Card/Delete", Name = "CardDelete")]
		public IActionResult CardDelete(string id)
		{
			try
			{
				var parameters = new Dictionary<string, string>
				{
					["cardNumber"] = id,
				};
				dynamic? res = Helper.Delete(this, "Card", null, Request.Cookies["sessionId"], TempData, parameters);
				return RedirectToRoute("CardIndex");
			}
			catch // unauthorized
			{
				return RedirectToRoute("Login");
			}
		}
	}
}
