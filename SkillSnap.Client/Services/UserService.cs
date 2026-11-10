using SkillSnap.Shared;
using System.Text.Json;
using System.Net.Http.Json;

namespace SkillSnap.Client.Services
{
    public class UserService
    {
        public readonly HttpClient _httpClient;

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<PortfolioUserDto>> GetAllUsersAsync()
        {
            try {
                var response = await _httpClient.GetAsync("server/user");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                // deserialize the json string into an array of type skill
                var skills = JsonSerializer.Deserialize<PortfolioUserDto[]>(json, options);
                return skills?.ToList() ?? new List<PortfolioUserDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching skills: {ex.Message}");
                return new List<PortfolioUserDto>();
            }
            
        }

        public async Task<PortfolioUserDto> GetUserByIdAsync(string id)
        {
            try
            {
                var url = $"server/user/{id}";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                // deserialize the json string into an array of type skill
                var user = JsonSerializer.Deserialize<PortfolioUserDto>(json, options);
                return user ?? new PortfolioUserDto();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching user: {ex.Message}");
                return new PortfolioUserDto();
            }
        }

        public async Task<SuccessResponseDto> UpdateUserByIdAsync(string id, PortfolioUserDto portfolioUserDto)
        {
            try
            {
                var url = $"server/user/{id}";
                Console.WriteLine("Url: " + url);
                var response = await _httpClient.PutAsJsonAsync(url, portfolioUserDto);

                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                // deserialize the json string into an array of type skill
                var successResponse = JsonSerializer.Deserialize<SuccessResponseDto>(json, options);

                return successResponse ?? new SuccessResponseDto { Success = false, Message = "Something went wrong."};
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching user: {ex.Message}");
                return new SuccessResponseDto { Success = false, Message = "Something went wrong."};
            }
        }
    }
}