using SkillSnap.Shared;
using System.Text.Json;
namespace SkillSnap.Client.Services;

public class ProjectServices
{
    private readonly HttpClient _httpClient;

    public ProjectServices(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<ProjectResponseDto>> GetProjectsAsync()
    {
        try {
            var response = await _httpClient.GetAsync("server/projects");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            // deserialize the json string into an array of type project
            var projects = JsonSerializer.Deserialize<ProjectResponseDto[]>(json, options);
            return projects?.ToList() ?? new List<ProjectResponseDto>();
        }
        catch (Exception ex)
        {
            // Handle exceptions (e.g., log the error)
            Console.WriteLine($"Error fetching projects: {ex.Message}");
            return new List<ProjectResponseDto>();
        }
    }

    public async Task<ProjectResponseDto?> CreateProjectAsync(CreateProjectDto createProjectDto)
    {
        try
        {
            var jsonContent = new StringContent(JsonSerializer.Serialize(createProjectDto), System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("server/projects/create", jsonContent);
            response.EnsureSuccessStatusCode();

            // Read the response content as a string
            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var createdProject = JsonSerializer.Deserialize<ProjectResponseDto>(json, options);
            
            return createdProject;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating project: {ex.Message}");
            return null;
        }
    }
}