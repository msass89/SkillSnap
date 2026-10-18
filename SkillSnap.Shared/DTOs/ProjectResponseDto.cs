using System.ComponentModel.DataAnnotations;
namespace SkillSnap.Shared;

public class ProjectResponseDto
{
    public string Title { get; set; }

    public string Description { get; set; } 

    public string? ImageUrl { get; set; }

}