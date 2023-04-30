using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalBankManagement.Models
{
	public class LoanApplicationModel
	{
		[Key]
		public int Id { get; set; }

		public int UserId { get; set; }
		[ForeignKey("UserId")]
		public UserModel User { get; set; }

		[Column(TypeName = "decimal(12, 2)")]
		public decimal Amount { get; set; }

		public int? DebitFrom { get; set; }
		[ForeignKey("DebitFrom")]
		public AccountModel DebitAccount { get; set; }

		public int Status { get; set; } // 0 - not yet reviewed; 1 - approved; -1 declined
	}
}
