using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DigitalBankManagement.Models
{
	public class RdAccountModel
	{
		[Key]
		public int AccountId { get; set; }
		[ForeignKey(nameof(AccountId))]
		public AccountModel Account { get; set; }

		[Column(TypeName = "decimal(12, 2)")]
		public decimal MonthlyDeposit { get; set; }

		public int Duration { get; set; }

		public int? DebitFrom { get; set; }
		[ForeignKey(nameof(DebitFrom))]
		public AccountModel DebitAccount { get; set; }
	}
}
