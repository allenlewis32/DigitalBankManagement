using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalBankManagement.Models
{
	public class AccountModel
	{
		[Key]
		public int Id { get; set; }

		public int UserId { get; set; }
		[ForeignKey("UserId")]
		public UserModel User { get; set; }

		[Column(TypeName = "decimal(12, 2)")]
		public decimal Amount { get; set; }

		public DateTime DateCreated { get; set; }

		public bool Active { get; set; }

		public int Type { get; set; }

		public const int TypeSavings = 0;
		public const int TypeFD = 1;
		public const int TypeRD = 2;
		public const int TypeLoan = 3;
	}
}
