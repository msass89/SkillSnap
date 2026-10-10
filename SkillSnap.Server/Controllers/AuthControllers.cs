using Microsoft.AspNetCore.Mvc;
using SkillSnap.Server.Models;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;


[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IConfiguration _configuration;

    public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> CreateUserAsync(string email, string password, string roleName)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return BadRequest("Please enter email and password");

        var user = new ApplicationUser { UserName = email, Email = email };

        var userCreateResult = await _userManager.CreateAsync(user, password);

        if(!userCreateResult.Succeeded)
            return BadRequest("User creation was not successfull");
        
        var addRoleResult = await _userManager.AddToRoleAsync(user, roleName);

        if(!addRoleResult.Succeeded)
            return BadRequest($"Failed to add a role to the user {user.UserName}");

        return Ok(new { Message = $"User {user.UserName} created successfully" });
    }

    [HttpPost("post")]
    public async Task<IResult> LoginAsync(string userName, string password)
    {
        var foundUser = await _userManager.FindByNameAsync(userName);

        if(foundUser == null)
            return Results.Unauthorized();

        var result = await _signInManager.PasswordSignInAsync(
                userName: userName,
                password: password,
                isPersistent: false,
                lockoutOnFailure: false
        );

        if (result.Succeeded)
        {
            return Results.Ok(new { token = GenerateTokenString(foundUser) });
        }
        else
            return Results.Unauthorized();
        
    }

    private async Task<string> GenerateTokenString(ApplicationUser user)
    {
        var userRoles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, userRoles.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return tokenString;
    }

}