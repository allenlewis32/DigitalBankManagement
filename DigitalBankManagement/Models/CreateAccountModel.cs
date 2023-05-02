namespace DigitalBankManagement.Models
{
	public class CreateAccountModel
	{
		public int Type {  get; set; }
		public int? DebitFrom { get; set; }
		public int? Duration { get; set; }
		public decimal? MonthlyDeposit { get; set; }
		public decimal? InitialDeposit { get; set; }
	}
}
