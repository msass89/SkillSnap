using System.ComponentModel.DataAnnotations;

namespace SkillSnap.Client.Models
{
    public class EditProfilePictureModel
    {
        [Url]
        public string ProfileImageUrl { get; set; } = "";
    }
    public class EditBioModel
    {
        [MaxLength(50, ErrorMessage = "Bio should be 50 characters at maximum.")]
        public string Bio { get; set; } = "";
    }

    public class EditSkillModel
    {
        [Required(ErrorMessage = "Skill name is required")]
        [MaxLength(20, ErrorMessage = "Skill name should be 20 characters at maximum.")]
        [RegularExpression("^[A-Za-z0-9 ]+$", ErrorMessage = "The skill name should only contain letters and numbers.")]
        public string SkillName { get; set; } = "";

        [Range(1, 5)]
        public int SkillLevel { get; set; } = 1;
    }

    public class EditProjectModel
    {
        [Required(ErrorMessage = "Project title is required")]
        [MaxLength(20, ErrorMessage = "Project title should be 20 characters at maximum.")]
        public string ProjectTitle { get; set; } = "";

        [Required(ErrorMessage = "Project description is required")]
        [MaxLength(200, ErrorMessage = "Project title should be 200 characters at maximum.")]
        public string ProjectDescription { get; set; } = "";
    }
}