using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalBankManagement.Models
{
	public class LoanApplicationModel
	{
		[Key]
		public int Id { get; set; }

		public int UserId { get; set; }
		[ForeignKey(nameof(UserId))]
		public UserModel User { get; set; }

		[Column(TypeName = "decimal(12, 2)")]
		public decimal Amount { get; set; }

		public int? DebitFrom { get; set; }
		[ForeignKey(nameof(DebitFrom))]
		public AccountModel DebitAccount { get; set; }

		public int Status { get; set; } // 0 - not yet reviewed; 1 - approved; -1 declined

		public int? LoanId { get; set; }
		[ForeignKey(nameof(LoanId))]
		public AccountModel LoanAccount { get; set; }

		public int Duration { get; set; }
	}
}
