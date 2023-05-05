using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalBankManagement.Models
{
	public class CardModel
	{
		[Key]
		[Column(TypeName = "decimal(16)")]
		public decimal Id { get; set; }
		public int? AccountId { get; set; }
		[ForeignKey(nameof(AccountId))]
		public AccountModel? Account { get; set; }
		public int Pin { get; set; }
		public DateTime Expiry { get; set; }
	}
}
