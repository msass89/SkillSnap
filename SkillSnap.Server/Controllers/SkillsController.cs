using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SkillSnap.Shared;

[ApiController]
[Route("server/skills")]
public class SkillsController : ControllerBase
{
    private readonly SkillSnapContext _context;
    private readonly IMemoryCache _memoryCache;
    private const string CacheKeySkills = "skills_all";

    public SkillsController(SkillSnapContext context, IMemoryCache memoryCache)
    {
        _context = context;
        _memoryCache = memoryCache;
    }

    //get all skills
    [HttpGet]
    public async Task<IActionResult> GetSkills()
    {
        if (!_memoryCache.TryGetValue(CacheKeySkills, out List<SkillResponseDto>? cachedSkillsDtoList))
        {
            // If the cache does not contain the skills, retrieve them from the database
            var items = await _context.Skills
                .AsNoTracking() // Use AsNoTracking for read-only queries to improve performance
                .Select(item => new SkillResponseDto
                {
                    Id = item.Id,
                    Name = item.Name,
                    Level = item.Level,
                    PortfolioUserId = item.PortfolioUserId
                }).ToListAsync();

            // Set cache options
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(30)); // Cache for 30 seconds

            // Save data in cache
            _memoryCache.Set(CacheKeySkills, items, cacheEntryOptions);
            cachedSkillsDtoList = items;
        }

        return Ok(cachedSkillsDtoList);
    }


    //create a new skill
    [HttpPost("create")]
    public async Task<IActionResult> CreateSkill([FromBody] CreateSkillDto createSkillDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var skill = new Skill
        {
            Name = createSkillDto.Name,
            Level = createSkillDto.Level,
            PortfolioUserId = createSkillDto.PortfolioUserId
        };
        _context.Skills.Add(skill);
        await _context.SaveChangesAsync();

        // return the created skill as a DTO
        SkillResponseDto skillResponseDto = new SkillResponseDto
        {
            Id = skill.Id,
            Name = skill.Name,
            Level = skill.Level,
            PortfolioUserId = skill.PortfolioUserId
        };

        // invalidate cached skills list after creating a new skill
        _memoryCache.Remove(CacheKeySkills);

        return Ok(skillResponseDto);
    }

    //delete a skill by id
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSkill(int id)
    {
        var item = await _context.Skills.FirstOrDefaultAsync(item => item.Id == id);
        if (item == null)
        {
            return NotFound();
        }
        _context.Skills.Remove(item);
        await _context.SaveChangesAsync();

        // invalidate cached skills list after deleting a skill
        _memoryCache.Remove(CacheKeySkills);

        return Ok($"Skill with ID {id} deleted successfully.");
    }
}