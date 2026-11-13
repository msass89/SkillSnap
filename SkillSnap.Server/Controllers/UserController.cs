using Microsoft.AspNetCore.Mvc;
using SkillSnap.Server.Models;
using Microsoft.AspNetCore.Identity;
using SkillSnap.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.Extensions.Caching.Memory;


[ApiController]
[Route("server/user")]
public class UserController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMemoryCache _memoryCache;
    private const string CacheKeyUserPrefix = "user_";

    public UserController(UserManager<ApplicationUser> userManager, IMemoryCache memoryCache)
    {
        _userManager = userManager;
        _memoryCache = memoryCache;
    }
    

    [HttpGet]
    public async Task<IActionResult> GetAllUsersAsync()
    {
        if (!_memoryCache.TryGetValue(CacheKeyUserPrefix + "all", out List<PortfolioUserDto>? cachedUserDtoList))
        {
            var userList = await _userManager.Users
                        .AsNoTracking()
                        .Select(user => new PortfolioUserDto
                        {
                            UserId = user.Id,
                            FullName = user.FullName,
                            Bio = user.Bio,
                            ProfileImageUrl = user.ProfileImageUrl
                        })
                        .ToListAsync();

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(10)); // Cache sliding expiration for 30 minutes

            // Save data in cache
            _memoryCache.Set(CacheKeyUserPrefix + "all", userList, cacheEntryOptions);
        }

        return Ok(cachedUserDtoList);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        PortfolioUserDto portfolioUserDto = new PortfolioUserDto
        {
            FullName = user.FullName,
            Bio = user.Bio,
            ProfileImageUrl = user.ProfileImageUrl
        };

        return Ok(portfolioUserDto);
    }

    [HttpPost("register")]
    public async Task<IActionResult> CreateUserAsync([FromBody] RegisterRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest("Please enter email and password");

        var user = new ApplicationUser { UserName = request.Email, Email = request.Email, FullName = request.FullName };

        var userCreateResult = await _userManager.CreateAsync(user, request.Password);

        if(!userCreateResult.Succeeded)
            return BadRequest("User creation failed");
        
        var addRoleResult = await _userManager.AddToRoleAsync(user, "User");

        if(!addRoleResult.Succeeded)
            return BadRequest($"Failed to add a role to the user {user.UserName}");

        _memoryCache.Remove(CacheKeyUserPrefix + "all");

        return Ok(new { Message = $"User {user.UserName} created successfully" });
    }

    [Authorize]
    [HttpPut]
    public async Task<IActionResult> UpdateUserById([FromBody] PortfolioUserDto portfolioUser)
    {
         //every user can only update it's own profile
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (currentUserId != portfolioUser.UserId)
            return Forbid();

        var user = await _userManager.FindByIdAsync(portfolioUser.UserId);

        if(user == null)
            return NotFound(new SuccessResponseDto { Success = false, Message = "Something went wrong"});

        user.FullName = portfolioUser.FullName;
        user.Bio = portfolioUser.Bio;
        user.ProfileImageUrl = portfolioUser.ProfileImageUrl;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
            return BadRequest(new SuccessResponseDto { Success = false, Message = "Bad request" });
        
        _memoryCache.Remove(CacheKeyUserPrefix + "all");

        return Ok(new SuccessResponseDto { Success = true, Message = "User updated successfully." });
    }
}