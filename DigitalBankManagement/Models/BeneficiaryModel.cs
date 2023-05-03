using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalBankManagement.Models
{
	public class BeneficiaryModel
	{
		[Key]
		public int Id { get; set; }
		public int? UserId { get; set; }
		[ForeignKey(nameof(UserId))]
		public UserModel? User { get; set; }
		public string Name { get; set; }
		public int? BeneficiaryAccountId { get; set; }
		[ForeignKey(nameof(BeneficiaryAccountId))]
		public AccountModel? BeneficiaryAccount { get; set; }
	}
}
