using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalBankManagement.Models
{
	public class SessionModel
	{
		[Key]
		public string SessionId { get; set; }
		public int UserId { get; set; }
		[ForeignKey(nameof(UserId))]
		public UserModel User { get; set; }
		public DateTime LastUsed { get; set; }
	}
}
