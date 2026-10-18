
using SkillSnap.Shared;

namespace SkillSnap.Server.Models
{
    public class PortfolioUserDto
    {
        public string FullName { get; set; }

        public string? Bio { get; set; } 

        public string ProfileImageUrl { get; set; }

        public List<ProjectResponseDto>? Projects { get; set; }
        public List<SkillResponseDto>? Skills { get; set; } 
    }
}


