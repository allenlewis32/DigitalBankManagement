using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalBankManagement.Models
{
	public class TransactionModel
	{
		[Key]
		public int Id { get; set; }

		public int? FromAccountId { get; set; }
		[ForeignKey(nameof(FromAccountId))]
		public AccountModel? FromAccount { get; set; }
		public int? ToAccountId { get; set; }
		[ForeignKey(nameof(ToAccountId))]
		public AccountModel? ToAccount { get; set; }
		public DateTime Time { get; set; }
		[Column(TypeName = "decimal(12, 2)")]
		public decimal Amount { get; set; }
	}
}
