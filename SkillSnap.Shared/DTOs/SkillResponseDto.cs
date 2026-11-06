namespace SkillSnap.Shared;

public class SkillResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Level { get; set; }

    public string? Message { get; set; }

    public bool Success { get; set; } = false;
}