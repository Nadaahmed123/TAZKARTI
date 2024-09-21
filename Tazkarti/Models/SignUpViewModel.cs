using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tazkarti.Models
{
    public class SignUpViewModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Format for email")]
        public string Email { get; set; }

        [Required]
        [MaxLength(8)]
        public string Password { get; set; }

        [Required]
        [MaxLength(8)]
        [Compare(nameof(Password), ErrorMessage = "Password Mismatch")]
        public string ConfirmPassword { get; set; }

        [Required]
        public bool IsAgree { get; set; }
    }
}
