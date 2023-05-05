using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalBankManagement.Models
{
	public class LoanModel
	{
		[Key]
		public int AccountId { get; set; }
		[ForeignKey(nameof(AccountId))]
		public AccountModel Account { get; set; }

		[Column(TypeName = "decimal(12, 2)")]
		public decimal Emi { get; set; }

		public int Duration { get; set; }

		public int? DebitFrom { get; set; }
		[ForeignKey(nameof(DebitFrom))]
		public AccountModel DebitAccount { get; set; }
	}
}
