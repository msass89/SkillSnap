using SkillSnap.Shared;
using System.Text.Json;
namespace SkillSnap.Client.Services;

public class SkillServices
{
    private readonly HttpClient _httpClient;

    public SkillServices(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<SkillResponseDto>> GetSkillsAsync()
    {
        try {
            var response = await _httpClient.GetAsync("server/skills");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            // deserialize the json string into an array of type skill
            var skills = JsonSerializer.Deserialize<SkillResponseDto[]>(json, options);
            return skills?.ToList() ?? new List<SkillResponseDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching skills: {ex.Message}");
            return new List<SkillResponseDto>();
        }
    }

    public async Task<SkillResponseDto?> CreateSkillAsync(CreateSkillDto createSkillDto)
    {
        try
        {
            var jsonContent = new StringContent(JsonSerializer.Serialize(createSkillDto), System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("server/skills/create", jsonContent);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            // deserialize the json string into an object of type skill if successful
            var createdSkill = JsonSerializer.Deserialize<SkillResponseDto>(json, options);
            return createdSkill;
        }
        catch (Exception ex)
        {
            // Handle exceptions (e.g., log the error)
            Console.WriteLine($"Error creating skill: {ex.Message}");
            return null;
        }
    }
}