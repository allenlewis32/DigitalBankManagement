﻿using Microsoft.AspNetCore.Mvc;
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
						case -1:
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
						statusTextClass,
						Id = item["id"]
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

		[HttpPost]
		[Route("/LoanApplication/Apply", Name = "LoanApply")]
		public IActionResult LoanApply(string debitFrom, string amount, string duration)
		{
			try
			{
				var parameters = new
				{
					debitFrom,
					amount,
					duration
				};
				dynamic? res = Helper.Post(this, "Account", "ApplyLoan", Request.Cookies["sessionId"], TempData, parameters);
				return RedirectToRoute("LoanApplicationIndex");
			}
			catch // unauthorized
			{
				return RedirectToRoute("Login");
			}
		}

		[Route("/LoanApplication/Approve", Name = "LoanApprove")]
		public IActionResult LoanApprove(string id)
		{
			try
			{
				var parameters = new
				{
					applicationId = id,
				};
				dynamic? res = Helper.Post(this, "LoanApplication", "Approve", Request.Cookies["sessionId"], TempData, parameters);
				return RedirectToRoute("LoanApplicationIndex");
			}
			catch // unauthorized
			{
				return RedirectToRoute("Login");
			}
		}

		[Route("/LoanApplication/Reject", Name = "LoanReject")]
		public IActionResult LoanReject(string id)
		{
			try
			{
				var parameters = new
				{
					applicationId = id,
				};
				dynamic? res = Helper.Post(this, "LoanApplication", "Reject", Request.Cookies["sessionId"], TempData, parameters);
				return RedirectToRoute("LoanApplicationIndex");
			}
			catch // unauthorized
			{
				return RedirectToRoute("Login");
			}
		}
	}
}
