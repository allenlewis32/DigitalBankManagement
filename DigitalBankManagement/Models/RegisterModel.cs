using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalBankManagement.Models
{
	public class RegisterModel
	{
		public string Email { get; set; }

		public string Password { get; set; }

		public string FirstName { get; set; }

		public string LastName { get; set; }

		public string Role { get; set; }

		public string? SessionId {  get; set; }
	}
}
