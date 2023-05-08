using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace DigitalBankManagement.Controllers
{
	public class LoanApplicationController : Controller
	{
		[Route("/LoanApplication/Index", Name = "LoanApplicationIndex")]
		public IActionResult Index()
		{
			try
			{
				dynamic? res = Helper.Get(this, "LoanApplication", null, Request.Cookies["sessionId"], TempData);
				var applications = new List<dynamic>();
				foreach (var item in (JArray)res!)
				{
					var status = "";
					var statusTextClass = "";
					switch ((int)item["status"]!)
					{
						case 0:
							status = "Pending";
							break;
						case 1:
							status = "Approved";
							statusTextClass = "text-success";
							break;
						case 2:
							status = "Declined";
							statusTextClass = "text-danger";
							break;
					}
					applications.Add(new
					{
						amount = item["amount"],
						debitFrom = item["debitFrom"],
						duration = item["duration"],
						status,
						statusTextClass
					});
				}
				ViewData["applications"] = applications;
				return View();
			}
			catch // unauthorized
			{
				return RedirectToRoute("Login");
			}
		}
	}
}
