using System.ComponentModel.DataAnnotations;
namespace SkillSnap.Shared;

// DTO for creating a new skill, including name and level
public class CreateSkillDto
{
    [Required, MaxLength(50)]
    public string Name { get; set; }

    [Required, MaxLength(50)]
    public string Level { get; set; }

    [Required]
    public int PortfolioUserId { get; set; }
}