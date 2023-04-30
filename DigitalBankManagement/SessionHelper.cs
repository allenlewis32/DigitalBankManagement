using System.Security.Cryptography;
using DigitalBankManagement.Data;
using DigitalBankManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace DigitalBankManagement
{
	public class SessionHelper
	{
		private static RandomNumberGenerator rng = RandomNumberGenerator.Create();
		private static char[] chars = "abcdefghijklmnopqrstuvwxyz012345".ToCharArray();
		private static int len = 24;
		
		// creates a unique session id, stores it in db and returns the id
		public static string GenerateSessionId(ApplicationDbContext context, UserModel user)
		{
			DbSet<SessionModel> sessions = context.Sessions;
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
				SessionModel? session = sessions.FirstOrDefault(s => s.SessionId == sessionId);
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
					if(DateTime.UtcNow > session.LastUsed.AddMinutes(20)) // check whether the previous session has expired; 20 minutes of inactivity
					{
						sessions.Remove(session); // remove inactive session
					}
				}
			}
		}
	}
}
