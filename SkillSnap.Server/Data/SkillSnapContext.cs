using Microsoft.EntityFrameworkCore;
using SkillSnap.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using SkillSnap.Server.Models;

public class SkillSnapContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
{
    public SkillSnapContext(DbContextOptions<SkillSnapContext> options) : base(options) { }
    public DbSet<PortfolioUser> PortfolioUsers { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Skill> Skills { get; set; }
}