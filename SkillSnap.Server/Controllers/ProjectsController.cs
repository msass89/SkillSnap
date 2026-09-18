using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SkillSnap.Shared;

[ApiController]
[Route("server/projects")]
public class ProjectsController : ControllerBase
{
    private readonly SkillSnapContext _context;
    private readonly IMemoryCache _memoryCache;
    private const string CacheKeyProjects = "projects_all";

    public ProjectsController(SkillSnapContext context, IMemoryCache memoryCache)
    {
        _context = context;
        _memoryCache = memoryCache;
    }

    //get all projects
    [HttpGet]
    public async Task<IActionResult> GetProjects()
    {
        if (!_memoryCache.TryGetValue(CacheKeyProjects, out List<ProjectResponseDto>? cachedProjects))
        {
            // If the cache does not contain the projects, retrieve them from the database
            var items = await _context.Projects
                .AsNoTracking() // Use AsNoTracking for read-only queries to improve performance
                .Select(item => new ProjectResponseDto
                {
                    Id = item.Id,
                    Title = item.Title,
                    Description = item.Description,
                    ImageUrl = item.ImageUrl,
                    PortfolioUserId = item.PortfolioUserId
                }).ToListAsync();

            // Set cache options
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(30)); // Cache for 30 seconds

            // Save data in cache
            _memoryCache.Set(CacheKeyProjects, items, cacheEntryOptions);
            cachedProjects = items;
        }

        return Ok(cachedProjects);
    }


    //create a new project
    [HttpPost]
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectDto createProjectDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var project = new Project
        {
            Title = createProjectDto.Title,
            Description = createProjectDto.Description,
            PortfolioUserId = createProjectDto.PortfolioUserId
        };
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        // return the created project as a DTO
        ProjectResponseDto projectResponseDto = new ProjectResponseDto
        {
            Id = project.Id,
            Title = project.Title,
            Description = project.Description,
            ImageUrl = project.ImageUrl,
            PortfolioUserId = project.PortfolioUserId
        };

        // invalidate cached projects list after creating a new project
        _memoryCache.Remove(CacheKeyProjects);

        return Ok($"Project created successfully: {projectResponseDto.Title}");
    }

    //delete a project by id
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        var item = await _context.Projects.FirstOrDefaultAsync(item => item.Id == id);
        if (item == null)
        {
            return NotFound();
        }
        _context.Projects.Remove(item);
        await _context.SaveChangesAsync();

        // invalidate cached projects list after deleting a project
        _memoryCache.Remove(CacheKeyProjects);

        return Ok($"Project with ID {id} deleted successfully.");
    }
}