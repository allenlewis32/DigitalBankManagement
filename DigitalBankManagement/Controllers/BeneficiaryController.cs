using Microsoft.AspNetCore.Mvc;

namespace DigitalBankManagement.Controllers
{
	public class BeneficiaryController : Controller
	{
		[Route("/Beneficiary/Index", Name = "BeneficiaryIndex")]
		public IActionResult Index()
		{
			try
			{
				dynamic? beneficiaries = Helper.Get(this, "Beneficiary", null, Request.Cookies["sessionId"], TempData);
				ViewData["beneficiaries"] = beneficiaries;
				return View();
			}
			catch // unauthorized
			{
				return RedirectToRoute("Login");
			}
		}
	}
}
