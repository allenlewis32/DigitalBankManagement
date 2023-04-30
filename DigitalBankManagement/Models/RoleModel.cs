using System.ComponentModel.DataAnnotations;

namespace DigitalBankManagement.Models
{
	public class RoleModel
	{
		[Key]
		public int Id { get; set; }
		public string Name { get; set; }
	}
}
