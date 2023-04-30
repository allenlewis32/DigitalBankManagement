using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DigitalBankManagement.Models
{
	[Index(nameof(Email), IsUnique = true)]
	[Index(nameof(UserName), IsUnique = true)]
	public class UserModel
	{
		[Key]
		public int Id { get; set; }

		public string UserName { get; set; }

		public string Email { get; set; }

		public string PasswordHash { get; set; }

		public int RoleId { get; set; }
		[ForeignKey(nameof(RoleId))]
		public RoleModel Role { get; set; }

		public string FirstName { get; set; }

		public string LastName { get; set; }

		public bool Active { get; set; }
	}
}
