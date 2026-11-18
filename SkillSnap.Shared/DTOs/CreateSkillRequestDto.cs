using System.ComponentModel.DataAnnotations;
namespace SkillSnap.Shared;

public class CreateSkillRequestDto
{
    [Required]
    [MaxLength(30)]
    [RegularExpression("^[A-Za-z0-9 .]+$")]
    public string Name { get; set; }

    [Range(1, 5)]
    public int Level { get; set; }

    [Required]
    public string ApplicationUserId { get; set; }
}