using System.Security.Cryptography;
using DigitalBankManagement.Data;
using DigitalBankManagement.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

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
			if(session != null)
			{
				session.LastUsed = DateTime.UtcNow;
				context.Sessions.Update(session);
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

		// Returns user associated with a session
		public static UserModel? GetUser(ApplicationDbContext context, string sessionId)
		{
			SessionModel? session = GetSession(context, sessionId);
			UserModel? user = null;
			if (session != null)
			{
				user = context.Users.First(u => u.Id == session.UserId);
				user.Role = context.Roles.First(r => r.Id == user.RoleId);
			}
			return user;
		}
	}
}
