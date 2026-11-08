using SkillSnap.Shared;
using System.Text.Json;

namespace SkillSnap.Client.Services
{
    public class SkillService
    {
        private readonly HttpClient _httpClient;

        public SkillService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<SkillResponseDto>> GetSkillsByUserAsync(string userId)
        {
            try {
                var response = await _httpClient.GetAsync($"server/skills/{userId}");
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

        public async Task<SkillResponseDto> CreateSkillAsync(CreateSkillRequestDto createSkillDto)
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

                // deserialize the json string into an array of type skill
                var createSkillResponse = JsonSerializer.Deserialize<SkillResponseDto>(json, options);
                
                return createSkillResponse ?? new SkillResponseDto { Success = false, Message = "Something went wrong"};
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log the error)
                Console.WriteLine($"Error creating skill: {ex.Message}");
                return new SkillResponseDto { Success = false, Message = "Something went wrong"};
            }
        }

        public async Task<SuccessResponseDto> DeleteSkillByIdAsync (int skillId)
        {
            try
            {
                var url = $"server/skills/{skillId}";
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
}

