using System.Net;
using System.Security.Cryptography;
using System.Web;
using DigitalBankManagement.Controllers;
using DigitalBankManagement.Data;
using DigitalBankManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace DigitalBankManagement
{
	public class Helper
	{
		private static RandomNumberGenerator rng = RandomNumberGenerator.Create();
		private static char[] chars = "abcdefghijklmnopqrstuvwxyz012345".ToCharArray();
		private static int len = 24;

		// creates a unique session id, stores it in db and returns the id
		public static string GenerateSessionId(ApplicationDbContext context, UserModel user)
		{
			DbSet<SessionModel> sessions = context.Sessions;

			// if a session for the user exists, return it
			SessionModel? session = sessions.FirstOrDefault(s => s.UserId == user.Id);
			if (session != null)
			{
				session.LastUsed = DateTime.UtcNow;
				context.SaveChanges();
				return session.SessionId;
			}

			byte[] bytes = new byte[len];
			char[] tmp = new char[len];
			while (true)
			{
				rng.GetBytes(bytes);
				for (int i = 0; i < len; i++)
				{
					tmp[i] = chars[bytes[i] % chars.Length];
				}
				string sessionId = new string(tmp);
				session = sessions.FirstOrDefault(s => s.SessionId == sessionId);
				if (session == null) // check if the sessionId is unique
				{
					sessions.Add(new()
					{
						SessionId = sessionId,
						UserId = user.Id,
						LastUsed = DateTime.UtcNow,
					});
					context.SaveChanges();
					return sessionId;
				}
				else
				{
					RemoveIfExpired(context.Sessions, session);

				}
			}
		}

		// checks whether a session has expired and returns true if expired and false if not
		private static bool RemoveIfExpired(DbSet<SessionModel> sessions, SessionModel session)
		{
			if (DateTime.UtcNow > session.LastUsed.AddMinutes(20)) // check whether the previous session has expired; 20 minutes of inactivity
			{
				sessions.Remove(session); // remove inactive session
				return true;
			}
			return false;
		}

		// Returns the session specified by sessionId or returns null if sessionId is invalid or expired
		public static SessionModel? GetSession(ApplicationDbContext context, string sessionId)
		{
			SessionModel? session = context.Sessions.FirstOrDefault(s => s.SessionId == sessionId);
			if (session != null)
			{
				if (RemoveIfExpired(context.Sessions, session))
				{
					return null;
				}
				session.LastUsed = DateTime.UtcNow;
				context.SaveChanges();
			}
			return session;
		}

		// Returns user associated with a session and updates the lastUsed attribute; otherwise, return null
		public static UserModel? GetUser(ApplicationDbContext context, string sessionId)
		{
			SessionModel? session = GetSession(context, sessionId);
			UserModel? user = null;
			if (session != null)
			{
				user = context.Users.First(u => u.Id == session.UserId);
				user.Role = context.Roles.First(r => r.Id == user.RoleId);
				session.LastUsed = DateTime.UtcNow;
				context.SaveChanges();
			}
			return user;
		}

		public static IActionResult TransferMoney(ApplicationDbContext context, ControllerBase controller, AccountModel? debitFromAccount, decimal amount, AccountModel? creditToAccount, bool deactivate = false)
		{
			if (debitFromAccount == null && creditToAccount == null)
			{
				return controller.BadRequest();
			}

			if (debitFromAccount != null)
			{
				// for creating a new account, verify whether the debit account is a savings account
				if (!deactivate && debitFromAccount.Type != AccountModel.TypeSavings)
				{
					return controller.BadRequest("Money can be debited only from savings account");
				}

				// verify balance
				if (debitFromAccount.Amount < amount)
				{
					return controller.Conflict("Not enough amount");
				}
			}

			if (debitFromAccount != null)
			{
				debitFromAccount.Amount -= amount;
			}
			if (creditToAccount != null)
			{
				creditToAccount.Amount += amount;
			}

			// store transaction
			context.Transactions.Add(new()
			{
				FromAccount = debitFromAccount,
				ToAccount = creditToAccount,
				Time = DateTime.UtcNow,
				Amount = amount,
			});

			// Finally, save changes
			context.SaveChanges();
			return controller.Ok();
		}

		// POST method helper
		public static dynamic? Post(Controller callingController, string controller, string action, string? sessionId, ITempDataDictionary tempData, object? model = null)
		{
			Task<dynamic?> res = PerformMethod("post", callingController, controller, action, sessionId, tempData, model);
			res.Wait();
			return res.Result;
		}

		// GET method helper
		public static dynamic? Get(Controller callingController, string controller, string? action, string? sessionId, ITempDataDictionary tempData, object? model = null)
		{
			Task<dynamic?> res = PerformMethod("get", callingController, controller, action, sessionId, tempData, model);
			res.Wait();
			return res.Result;
		}

		// DELETE method helper
		public static dynamic? Delete(Controller callingController, string controller, string? action, string? sessionId, ITempDataDictionary tempData, object? model = null)
		{
			Task<dynamic?> res = PerformMethod("delete", callingController, controller, action, sessionId, tempData, model);
			res.Wait();
			return res.Result;
		}

		// Performs the required HTTP method and returns the value
		public static async Task<dynamic?> PerformMethod(string method, Controller callingController, string controller, string? action, string? sessionId, ITempDataDictionary tempData, object? model = null)
		{
			using var client = new HttpClient();
			string baseUrl = $"{callingController.Request.Scheme}://{callingController.Request.Host}{callingController.Request.PathBase}/api/{controller}/"; // get the base URL of this website
			client.BaseAddress = new Uri(baseUrl);
			client.DefaultRequestHeaders.Add("sessionId", sessionId);
			HttpResponseMessage? result = null;
			switch (method)
			{
				case "post":
					{
						var responseTask = client.PostAsJsonAsync(action, model);
						await responseTask;
						result = responseTask.Result;
						break;
					}
				case "get":
					{
						if (model != null)
						{
							var query = HttpUtility.ParseQueryString("");
							foreach (var item in (Dictionary<string, string>)model)
							{
								query[item.Key] = item.Value;
							}
							action += "?" + query.ToString();
						}
						var responseTask = client.GetAsync(action);
						await responseTask;
						result = responseTask.Result;
						break;
					}
				case "delete":
					{
						if (model != null)
						{
							var query = HttpUtility.ParseQueryString("");
							foreach (var item in (Dictionary<string, string>)model)
							{
								query[item.Key] = item.Value;
							}
							action += "?" + query.ToString();
						}
						var responseTask = client.DeleteAsync(action);
						await responseTask;
						result = responseTask.Result;
						break;
					}
			}

			if (result!.IsSuccessStatusCode)
			{
				var content = await result.Content.ReadAsStringAsync();
				dynamic json = JsonConvert.DeserializeObject(content)!;
				return json;
			}
			else if (result.StatusCode == HttpStatusCode.Unauthorized || sessionId == null) // if unauthorized, throw Exception to be caught by the controller
			{
				throw new Exception("Unauthorized");
			}
			else
			{
				tempData["errorMessage"] = await result.Content.ReadAsStringAsync();
			}
			return null;
		}
	}
}
