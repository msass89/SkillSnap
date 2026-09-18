using System.ComponentModel.DataAnnotations;
namespace SkillSnap.Shared;

public class SkillResponseDto
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Level { get; set; }
    public int PortfolioUserId { get; set; }
}