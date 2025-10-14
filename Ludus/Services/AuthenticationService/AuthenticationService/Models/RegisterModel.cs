using System.ComponentModel.DataAnnotations;

namespace AuthenticationService.Models
{
    public class RegisterModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The passwords are different")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string Mlb { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Surname { get; set; }
        public bool Enable2FA { get; internal set; } = true;
    }
}
