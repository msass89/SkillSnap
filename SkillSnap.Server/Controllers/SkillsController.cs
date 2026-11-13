using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SkillSnap.Shared;
using SkillSnap.Server.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

[Authorize]
[ApiController]
[Route("server/skills")]
public class SkillsController : ControllerBase
{
    private readonly SkillSnapContext _context;
    private readonly IMemoryCache _memoryCache;
    private const string CacheKeySkillsPrefix = "skills_";

    public SkillsController(SkillSnapContext context, IMemoryCache memoryCache)
    {
        _context = context;
        _memoryCache = memoryCache;
    }

    //get skills by userId
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetSkillsByUser(string userId)
    {
        if (!_memoryCache.TryGetValue(CacheKeySkillsPrefix + userId, out List<SkillResponseDto>? cachedSkillsDtoList))
        {
            // If the cache does not contain the skills, retrieve them from the database
            var skillItems = await _context.Skills
                .AsNoTracking() // Use AsNoTracking for read-only queries to improve performance
                .Where(skill => skill.ApplicationUserId == userId) //filter for a specific ApplicationUserId
                .Select(skill => new SkillResponseDto
                {
                    Id = skill.Id,
                    Name = skill.Name,
                    Level = skill.Level
                })
                .ToListAsync();

            // Set cache options
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(30)); // Cache for 30 seconds

            // Save data in cache
            _memoryCache.Set(CacheKeySkillsPrefix + userId, skillItems, cacheEntryOptions);
            cachedSkillsDtoList = skillItems;
        }

        return Ok(cachedSkillsDtoList);
    }


    //create a new skill
    [HttpPost("create")]
    public async Task<IActionResult> CreateSkill([FromBody] CreateSkillRequestDto createSkillDto)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (currentUserId != createSkillDto.ApplicationUserId)
            return Forbid();
            
        var skill = new Skill
        {
            Name = createSkillDto.Name,
            Level = createSkillDto.Level,
            ApplicationUserId = createSkillDto.ApplicationUserId
        };

        _context.Skills.Add(skill);
        int result = await _context.SaveChangesAsync();

        if(result > 0)
        {
            // invalidate cached skills list after creating a new skill
            _memoryCache.Remove(CacheKeySkillsPrefix + createSkillDto.ApplicationUserId);
            
            return Ok(new SkillResponseDto { Id = skill.Id, Name = skill.Name, Level = skill.Level, Success = true, Message = "Skill created successfully" });
        }
        else
        {
            return BadRequest(new SkillResponseDto{ Success = false, Message = "Something went wrong"});
        }
    }
    

    //delete a skill by id
    [HttpDelete("{skillId}")]
    public async Task<IActionResult> DeleteSkill(int skillId)
    {
        var skill = await _context.Skills.FirstOrDefaultAsync(skill => skill.Id == skillId);

        if (skill == null)
            return NotFound();

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (currentUserId != skill.ApplicationUserId)
            return Forbid();

        _context.Skills.Remove(skill);
        var result = await _context.SaveChangesAsync();

        if(result>0)
        {
            // invalidate cached skills list after deleting a skill
            _memoryCache.Remove(CacheKeySkillsPrefix + skill.ApplicationUserId);

            return Ok(new SuccessResponseDto{ Success = true, Message = "Skill deleted successfully."});
        }
        else
        {
            return BadRequest(new SuccessResponseDto{ Success = false, Message = "Something went wrong."});
        }
    }
}