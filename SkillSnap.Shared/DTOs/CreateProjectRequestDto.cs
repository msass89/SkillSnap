using System.ComponentModel.DataAnnotations;
namespace SkillSnap.Shared;

// DTO for creating a new project, including title and description
public class CreateProjectRequestDto
{
    [Required]
    [MaxLength(30)]
    public string Title { get; set; }

    [Required]
    [MaxLength(500)]
    public string Description { get; set; }

    //public string ImageUrl { get; set; }

    [Required]
    public string ApplicationUserId { get; set; }
}