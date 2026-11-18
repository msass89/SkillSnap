using System.ComponentModel.DataAnnotations;
namespace SkillSnap.Shared;

public class PortfolioUserDto
{
    [Required]
    public string UserId { get; set; }

    [Required]
    [RegularExpression(@"^[a-zA-Z]+(?: [a-zA-Z]+)+$")]
    public string FullName { get; set; }

    [MaxLength(100)]
    public string? Bio { get; set; } 

    [Url]
    public string? ProfileImageUrl { get; set; }
}