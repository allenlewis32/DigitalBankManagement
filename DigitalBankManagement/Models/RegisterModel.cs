using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace DigitalBankManagement.Models
{
    [Keyless]
    public class RegisterModel
    {
        [Required]
        [EmailAddress]
        [Display(Prompt = "Email")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Prompt = "Password")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password", Prompt = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Passwords don't match")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "First Name", Prompt = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name", Prompt = "Last Name")]
        public string LastName { get; set; }

        public string Role { get; set; } = "user";

        public string? SessionId { get; set; }
    }
}
