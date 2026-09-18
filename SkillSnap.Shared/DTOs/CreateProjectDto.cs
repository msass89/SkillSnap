using System.ComponentModel.DataAnnotations;
namespace SkillSnap.Shared;

// DTO for creating a new project, including title and description
public class CreateProjectDto
{
    [Required, MaxLength(50)]
    public string Title { get; set; }

    [Required, MaxLength(200)]
    public string Description { get; set; }

    [MaxLength(200)]
    public string ImageUrl { get; set; }

    [Required]
    public int PortfolioUserId { get; set; }
}