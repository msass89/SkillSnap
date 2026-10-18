/*using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SkillSnap.Shared;

public class Skill
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }
    public string Level { get; set; }

    [ForeignKey("PortfolioUser")]
    public string ApplicationUserId { get; set; }

    public ApplicationUser ApplicationUser { get; set; } 
}*/