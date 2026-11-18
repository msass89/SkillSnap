using System.ComponentModel.DataAnnotations;
namespace SkillSnap.Shared;

public class RegisterRequestDto
{
    [Required]
    [RegularExpression(@"^[a-zA-Z]+(?: [a-zA-Z]+)+$")]
    public string FullName { get; set; } = string.Empty;
    
    [Required]
    [DataType(DataType.EmailAddress)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [MinLength(6)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).+$")]
    public string Password { get; set; } = string.Empty;
}