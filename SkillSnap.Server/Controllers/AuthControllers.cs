using Microsoft.AspNetCore.Mvc;
using SkillSnap.Server.Models;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using SkillSnap.Shared;


[ApiController]
[Route("server/auth")]
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

        return Ok(new { Message = $"User {user.UserName} created successfully" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequestDto request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
            return Unauthorized(new TokenResponseDto { ErrorMessage = "Invalid Authentication" });

        var result = await _signInManager.PasswordSignInAsync(
                userName: user.UserName,
                password: request.Password,
                isPersistent: false,
                lockoutOnFailure: false
        );

        if (result.Succeeded)
        {
            var token = await GenerateTokenString(user);

            return Ok(new TokenResponseDto { IsAuthSuccessful = true, Token = token });
        }
        else
            return Unauthorized(new TokenResponseDto { ErrorMessage = "Invalid Authentication" });
    }

    private async Task<string> GenerateTokenString(ApplicationUser user)
    {
        var userRoles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Name, user.FullName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var role in userRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return await Task.FromResult(tokenString);
    }

}
