namespace SkillSnap.Shared;

public class TokenResponseDto
{
    public bool IsAuthSuccessful { get; set; }
    public string ErrorMessage { get; set; }
    public string Token { get; set; }
}