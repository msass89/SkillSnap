using System.ComponentModel.DataAnnotations;
namespace SkillSnap.Shared;

public class PortfolioUser
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    public string? Bio { get; set; } 

    public string ProfileImageUrl { get; set; } = "https://mockmind-api.uifaces.co/content/abstract/28.jpg";

    public List<Project>? Projects { get; set; }
    public List<Skill>? Skills { get; set; } 
}
