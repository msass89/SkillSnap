using Microsoft.AspNetCore.Identity;

namespace SkillSnap.Server.Models
{
    public class ApplicationUser : IdentityUser
    {
        //public string FullName { get; set; } = string.Empty;
        public string FullName { get; set; }

        public string? Bio { get; set; } 

        public string ProfileImageUrl { get; set; } = "https://mockmind-api.uifaces.co/content/abstract/28.jpg";

        public List<Project>? Projects { get; set; }
        public List<Skill>? Skills { get; set; } 
    }
}