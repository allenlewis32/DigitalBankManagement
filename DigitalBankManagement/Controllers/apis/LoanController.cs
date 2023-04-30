using DigitalBankManagement.Data;
using DigitalBankManagement.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DigitalBankManagement.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class LoanController : ControllerBase
	{
		private readonly ApplicationDbContext _context;
		public LoanController(ApplicationDbContext context)
		{
			_context = context;
		}

		[HttpGet]
		[ResponseCache(Duration =0)]
		public IActionResult Get([FromHeader] string sessionId)
		{
			UserModel? user = Helper.GetUser(_context, sessionId);
			if (user == null)
			{
				return Unauthorized();
			}
			IQueryable<LoanApplicationModel> applications = _context.LoanApplications;
			if (user.Role.Name.ToLower() == "user")
			{
				applications = applications.Where(application => application.UserId == user.Id);
				// don't send user information loaded while using where clause
				applications.ForEachAsync(application =>
				{
					application.User = null;
				}).Wait();
			}
			return Ok(applications);
		}
	}
}
