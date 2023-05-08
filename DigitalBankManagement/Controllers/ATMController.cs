using Microsoft.AspNetCore.Mvc;

namespace DigitalBankManagement.Controllers
{
	public class ATMController : Controller
	{
		[Route("/ATM/", Name = "ATM")]
		public IActionResult Index()
		{
			return View();
		}

		[HttpPost]
		[Route("/ATM/ATMTransact", Name = "ATMTransact")]
		public IActionResult ATMTransact(string action, string cardNumber, string pin, string amount)
		{
			try
			{
				var parameters = new
				{
					cardNumber,
					pin,
					amount
				};
				switch (action)
				{
					case "deposit":
						Helper.Post(this, "ATM", "Credit", null, TempData, parameters);
						TempData["successMessage"] = "Money deposited successfully";
						break;
					case "withdraw":
						Helper.Post(this, "ATM", "Debit", null, TempData, parameters);
						TempData["successMessage"] = "Money withdrawn successfully";
						break;
					default:
						TempData["errorMessage"] = "Invalid option";
						break;
				}
			}
			catch // Invalid pin
			{
				TempData["errorMessage"] = "Invalid pin";
			}
			return RedirectToRoute("ATM");
		}
	}
}
