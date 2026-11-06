using System.ComponentModel.DataAnnotations;
namespace SkillSnap.Shared;

public class ProjectResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; }

    public string Description { get; set; } 

    public string? ImageUrl { get; set; }
    public string? Message { get; set; }

    public bool Success { get; set; } = false;
}