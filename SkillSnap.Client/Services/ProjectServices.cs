using SkillSnap.Shared;
using System.Net.Http.Json;
using System.Text.Json;
namespace SkillSnap.Client.Services;

public class ProjectServices
{
    private readonly HttpClient _httpClient;

    public ProjectServices(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<ProjectResponseDto>> GetProjectsAsync(string userId)
    {
        try {
            var response = await _httpClient.GetAsync($"server/projects/{userId}");
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

    public async Task<ProjectResponseDto> CreateProjectAsync(CreateProjectRequestDto createProjectRequestDto)
    {
        try
        {
            var jsonContent = new StringContent(JsonSerializer.Serialize(createProjectRequestDto), System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("server/projects/create", jsonContent);

            Console.WriteLine("Response status code " + response.StatusCode);
            
            response.EnsureSuccessStatusCode();

            // Read the response content as a string
            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var createdProject = JsonSerializer.Deserialize<ProjectResponseDto>(json, options);
            
            return createdProject ?? new ProjectResponseDto { Success = false, Message = "Something went wrong"};
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating project: {ex.Message}");
            return new ProjectResponseDto { Success = false, Message = "Something went wrong"};
        }
    }

    public async Task<SuccessResponseDto> DeleteProjectByIdAsync (int projectlId)
        {
            try
            {
                var url = $"server/projects/{projectlId}";
                var response = await _httpClient.DeleteAsync(url);

                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                // deserialize the json string into an array of type skill
                var responseDto = JsonSerializer.Deserialize<SuccessResponseDto>(json, options);

                return responseDto ?? new SuccessResponseDto { Success = false, Message = "Something went wrong."};
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log the error)
                Console.WriteLine($"Error creating skill: {ex.Message}");
                return new SuccessResponseDto { Success = false, Message = "Something went wrong."};
            }
        }
    
}