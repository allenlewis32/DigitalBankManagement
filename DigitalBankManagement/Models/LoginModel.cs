using System.ComponentModel.DataAnnotations;

namespace DigitalBankManagement.Models
{
	public class LoginModel
	{
		[Required]
		[Display(Prompt = "Username")]
		public string UserName { get; set; }
		[Required]
		[DataType(DataType.Password)]
		[Display(Prompt = "Password")]
		public string Password { get; set; }
	}
}
