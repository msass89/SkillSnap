using System.ComponentModel.DataAnnotations;

﻿namespace SkillSnap.Client.Models
{
    public class CompleteProfileModel
    {
        [MaxLength(20, ErrorMessage = "Bio should be at maximum of 20 characters.")]
        public string Bio { get; set; } = "";
    }
}