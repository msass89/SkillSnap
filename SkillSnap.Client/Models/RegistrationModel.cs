using System.ComponentModel.DataAnnotations;

namespace SkillSnap.Client.Models
{
    public class RegistrationModel
    {
        [Required(ErrorMessage = "Full name is required.")]
        [RegularExpression(@"^[a-zA-Z]+(?: [a-zA-Z]+)+$", 
        ErrorMessage = "Please enter your full name (first and last).")]
        public string FullName { get; set; } = "";

        [Required(ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        [RegularExpression(
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).+$",
        ErrorMessage = "Password must contain an uppercase letter, a lowercase letter, a digit, and a special character.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }
        
    }
}