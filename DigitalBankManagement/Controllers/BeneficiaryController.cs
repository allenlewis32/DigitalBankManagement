using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

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

		[HttpPost]
		[Route("/Beneficiary/AddBeneficiary", Name = "AddBeneficiary")]
		public IActionResult AddBeneficiary(string name, string accountId)
		{
			try
			{
				var parameters = new
				{
					name,
					accountId,
				};
				dynamic? res = Helper.Post(this, "Beneficiary", null, Request.Cookies["sessionId"], TempData, parameters);
				return RedirectToRoute("BeneficiaryIndex");
			}
			catch // unauthorized
			{
				return RedirectToRoute("Login");
			}
		}

		[HttpGet]
		[Route("/Beneficiary/BeneficiaryDelete", Name = "BeneficiaryDelete")]
		public IActionResult BeneficiaryDelete(string id)
		{
			try
			{
				var parameters = new Dictionary<string, string>
				{
					["beneficiaryId"] = id,
				};
				dynamic? res = Helper.Delete(this, "Beneficiary", null, Request.Cookies["sessionId"], TempData, parameters);
				return RedirectToRoute("BeneficiaryIndex");
			}
			catch // unauthorized
			{
				return RedirectToRoute("Login");
			}
		}
	}
}
