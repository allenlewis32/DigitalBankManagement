using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DigitalBankManagement.Models
{
	[Keyless]
	public class InterestModel
	{
		[Column(TypeName = "decimal(5, 2)")]
		public decimal Savings { get; set; }
		[Column(TypeName = "decimal(5, 2)")]
		public decimal FD { get; set; }
		[Column(TypeName = "decimal(5, 2)")]
		public decimal RD { get; set; }
		[Column(TypeName = "decimal(5, 2)")]
		public decimal Loan { get; set; }
	}
}
