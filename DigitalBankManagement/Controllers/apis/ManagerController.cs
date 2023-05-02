using DigitalBankManagement.Data;
using DigitalBankManagement.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DigitalBankManagement.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ManagerController : ControllerBase
	{
		private readonly ApplicationDbContext _context;
		public ManagerController(ApplicationDbContext context)
		{
			_context = context;
		}

		[HttpGet]
		public IActionResult Get([FromHeader] string sessionId)
		{
			try
			{
				var user = Helper.GetUser(_context, sessionId);
				if (user == null || user.Role.Name != "admin")
				{
					return Unauthorized();
				}
				var managers = new List<UserModel>();
				_context.Users.Include(user => user.Role)
					.Where(user => user.Active && user.Role.Name.Equals("manager"))
					.ForEachAsync(user =>
					{
						// create new manager classes as we shouldn't be returning sensitive information
						managers.Add(new()
						{
							Id = user.Id,
							Email = user.Email,
							FirstName = user.FirstName,
							LastName = user.LastName,
							Active = true,
						});
					})
					.Wait();
				return Ok(managers);
			}
			catch
			{
				return Problem();
			}
		}

		[HttpPost]
		[Route("Disable")]
		public IActionResult Disable([FromHeader] string sessionId, [FromForm] int managerId)
		{
			try
			{
				var user = Helper.GetUser(_context, sessionId);
				if (user == null || user.Role.Name != "admin")
				{
					return Unauthorized();
				}
				_context.Users.First(user => user.Id == managerId).Active = false;
				_context.SaveChanges();
				return Ok();
			}
			catch
			{
				return Problem();
			}
		}
	}
}
