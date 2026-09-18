using System.ComponentModel.DataAnnotations;
namespace SkillSnap.Shared;

public class ProjectResponseDto
{
    [Key]
    public int Id { get; set; }

    public string Title { get; set; }

    public string Description { get; set; } 

    public string? ImageUrl { get; set; }

    public int PortfolioUserId { get; set; }
}