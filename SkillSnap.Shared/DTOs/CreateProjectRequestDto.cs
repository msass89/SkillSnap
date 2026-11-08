
namespace SkillSnap.Shared;

// DTO for creating a new project, including title and description
public class CreateProjectRequestDto
{
    public string Title { get; set; }

    public string Description { get; set; }

    public string ImageUrl { get; set; }

    public string ApplicationUserId { get; set; }
}