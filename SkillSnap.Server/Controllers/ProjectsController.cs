using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SkillSnap.Server.Models;
using SkillSnap.Shared;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

[Authorize]
[ApiController]
[Route("server/projects")]
public class ProjectsController : ControllerBase
{
    private readonly SkillSnapContext _context;
    private readonly IMemoryCache _memoryCache;
    //private const string CacheKeyProjects = "projects_all";

    public ProjectsController(SkillSnapContext context, IMemoryCache memoryCache)
    {
        _context = context;
        _memoryCache = memoryCache;
    }

    //get all projects
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetProjectsByUserId(string userId)
    {
        if (!_memoryCache.TryGetValue(userId, out List<ProjectResponseDto>? cachedProjectsDtoList))
        {
            // If the cache does not contain the projects, retrieve them from the database
            var items = await _context.Projects
                .AsNoTracking() // Use AsNoTracking for read-only queries to improve performance
                .Where(project => project.ApplicationUserId == userId) //filter for a specific ApplicationUserId
                .Select(project => new ProjectResponseDto
                {
                    Id = project.Id,
                    Title = project.Title,
                    Description = project.Description,
                    ImageUrl = project.ImageUrl
                }).ToListAsync();

            // Set cache options
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(30)); // Cache for 30 seconds

            // Save data in cache
            _memoryCache.Set(userId, items, cacheEntryOptions);
            cachedProjectsDtoList = items;
        }

        return Ok(cachedProjectsDtoList);
    }


    //create a new project
    [HttpPost("create")]
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectRequestDto createProjectDto)
    {
        //user are only allowed to create projects for their own profile
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (currentUserId != createProjectDto.ApplicationUserId)
            return Forbid();

        /*if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }*/

        var project = new Project
        {
            Title = createProjectDto.Title,
            Description = createProjectDto.Description,
            ApplicationUserId = createProjectDto.ApplicationUserId
        };

        _context.Projects.Add(project);
        int result = await _context.SaveChangesAsync();

        if(result > 0)
        {
            // invalidate cached projects list after creating a new project
            _memoryCache.Remove(createProjectDto.ApplicationUserId);

            // return the created project as a DTO
            ProjectResponseDto projectResponseDto = new ProjectResponseDto
            {
                Id = project.Id,
                Title = project.Title,
                Description = project.Description,
                ImageUrl = project.ImageUrl,
                Success = true,
                Message = "Project created successfully"
            };

            return Ok(projectResponseDto);
        }
        else
        {
            Console.WriteLine("No success");
            return BadRequest(new SkillResponseDto { Success = false, Message = "Something went wrong"});
        }
        
    }

    //delete a project by id
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        //users are only allowed the delete projects for their own profile
        var project = await _context.Projects.FirstOrDefaultAsync(project => project.Id == id);

        if (project == null)
            return NotFound();

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (currentUserId != project.ApplicationUserId)
            return Forbid();
        
        _context.Projects.Remove(project);
        var result = await _context.SaveChangesAsync();

        if(result > 0)
        {
            // invalidate cached projects list after deleting a project
            _memoryCache.Remove(project.ApplicationUserId);

            return Ok(new SuccessResponseDto{ Success = true, Message = $"Project deleted successfully."});
        }
        else
        {
            return BadRequest(new SuccessResponseDto{ Success = false, Message = "Something went wrong"});
        }
    }
}