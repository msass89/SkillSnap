using Microsoft.AspNetCore.Mvc;
using SkillSnap.Server.Models;
using Microsoft.AspNetCore.Identity;
using SkillSnap.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

[Authorize]
[ApiController]
[Route("server/user")]
public class UserController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    
    [HttpGet]
    public async Task<IActionResult> GetAllUsersAsync()
    {
        var userList = await _userManager.Users
                    .AsNoTracking()
                    .Select(user => new PortfolioUserDto
                    {
                        FullName = user.FullName,
                        Bio = user.Bio,
                        ProfileImageUrl = user.ProfileImageUrl
                    })
                    .ToListAsync();

        return Ok(userList);
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

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUserById(string id, [FromBody] PortfolioUserDto portfolioUser)
    {
         //every user can only update it's own profile
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (currentUserId != id)
            return Forbid();

        var user = await _userManager.FindByIdAsync(id);

        if(user != null)
        {
            user.FullName = portfolioUser.FullName;
            user.Bio = portfolioUser.Bio;
            user.ProfileImageUrl = portfolioUser.ProfileImageUrl;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            else
            {
                return Ok();
            }      
        }

        return NotFound();
    }
}