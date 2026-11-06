
namespace SkillSnap.Shared;

// DTO for creating a new skill, including name and level
public class CreateSkillRequestDto
{
    public string Name { get; set; }

    public int Level { get; set; }

    public string ApplicationUserId { get; set; }
}