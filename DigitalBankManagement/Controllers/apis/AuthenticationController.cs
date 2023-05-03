using System.Security.Cryptography;
using DigitalBankManagement.Data;
using DigitalBankManagement.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DigitalBankManagement.Controllers.apis
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthenticationController : ControllerBase
	{
		private ApplicationDbContext _context;
		private readonly ILogger<AuthenticationController> _logger;
		private static PasswordHasher<UserModel> _passwordHasher = new();
		public AuthenticationController(ApplicationDbContext context, ILogger<AuthenticationController> logger)
		{
			_context = context;
			_logger = logger;
		}

		[HttpPost]
		[Route("Register")]
		public IActionResult Register([FromForm] RegisterModel registerModel)
		{
			try
			{
				// authorize
				switch(registerModel.Role.ToLower())
				{
					case null:
						return Problem("Role required");
					case "admin": // admin can't be created using API
						return Problem("Cannot create admin account");
					case "manager": // manager can be created only by admin
						bool authorized = false;
						if(registerModel.SessionId != null)
						{
							// verify session and user role
							UserModel? callingUser = Helper.GetUser(_context, registerModel.SessionId);
							if(callingUser != null && callingUser.Role.Name.ToLower() == "admin") {
								authorized = true;
							}
						}
						if(!authorized)
						{
							return Unauthorized();
						}
						break;
				}
				if (_context.Users.FirstOrDefault(u => u.Email == registerModel.Email) != null)
				{
					return Conflict("User already exists");
				}
				UserModel user = new()
				{
					UserName = registerModel.Email,
					Email = registerModel.Email,
					RoleId = _context.Roles.FirstOrDefault(r => r.Name == registerModel.Role)!.Id,
					FirstName = registerModel.FirstName,
					LastName = registerModel.LastName,
					Active = true,
				};
				user.PasswordHash = _passwordHasher.HashPassword(user, registerModel.Password);
				_context.Users.Add(user);
				_context.SaveChanges();
				return Ok(GetSessionIdAndRole(_context, user));
			}
			catch
			{
				return Problem("Unable to create user");
			}
		}

		private static object GetSessionIdAndRole(ApplicationDbContext _context, UserModel user)
		{
			object sessionIdAndRole = new { sessionId = Helper.GenerateSessionId(_context, user), role = _context.Roles.First(r => r.Id == user.RoleId).Name };
			return sessionIdAndRole;
		}

		[HttpPost]
		[Route("Login")]
		public IActionResult Login([FromForm] LoginModel loginModel)
		{
			try
			{
				UserModel? user = _context.Users.FirstOrDefault(u => u.UserName == loginModel.UserName && u.Active);
				if(user == null) // check whether user exists
				{
					return Unauthorized("User not found");
				}
				var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginModel.Password);
				if(result == PasswordVerificationResult.Failed) // check whether password is valid
				{
					return Unauthorized("Invalid password");
				}
				return Ok(GetSessionIdAndRole(_context, user));
			}
			catch
			{
				return Problem("Unable to login");
			}
		}
		
		[HttpGet]
		[Route("Logout")]
		public IActionResult Logout([FromHeader] string sessionId)
		{
			try
			{
				SessionModel? session = _context.Sessions.FirstOrDefault(s => s.SessionId == sessionId);
				if(session == null)
				{
					return Unauthorized("Invalid session id");
				}
				_context.Sessions.Remove(session);
				_context.SaveChanges();
				return Ok();
			}
			catch
			{
				return Problem("Unable to logout");
			}
		}
	}
}
