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
                var users = JsonSerializer.Deserialize<PortfolioUserDto[]>(json, options);
                return users?.ToList() ?? new List<PortfolioUserDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching user: {ex.Message}");
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

        public async Task<bool> RegisterAsync(string fullName, string email, string password)
        {
            try
            {
                RegisterRequestDto registerRequest = new RegisterRequestDto
                {
                    FullName = fullName,
                    Email = email,
                    Password = password
                };

                HttpResponseMessage response = await _httpClient.PostAsJsonAsync(
                    "http://localhost:5029/server/user/register",
                    registerRequest
                );

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    string error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error: {response.StatusCode} - {error}");
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception accured: {e.Message}");
                return false;
            }
        }

        public async Task<SuccessResponseDto> UpdateUserByIdAsync(PortfolioUserDto portfolioUserDto)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync("server/user", portfolioUserDto);

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