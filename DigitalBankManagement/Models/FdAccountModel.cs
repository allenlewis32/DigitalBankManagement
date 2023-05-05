using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalBankManagement.Models
{
	public class FdAccountModel
	{
		[Key]
		public int AccountId { get; set; }
		[ForeignKey(nameof(AccountId))]
		public AccountModel Account { get; set; }

		[Column(TypeName = "decimal(12, 2)")]
		public decimal InitialDeposit { get; set; }
		public int Duration { get; set; }
	}
}
