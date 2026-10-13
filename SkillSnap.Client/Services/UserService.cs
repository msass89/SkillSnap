
using System.Net.Http.Json;
using SkillSnap.Shared;

namespace SkillSnap.Client.Services
{
    public class UserService
    {
        private readonly HttpClient _httpClient;

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> CreateUser(string fullName, string email, string password)
        {
            try
            {
                RegisterRequest registerRequest = new RegisterRequest
                {
                    FullName = fullName,
                    Email = email,
                    Password = password,
                    RoleName = "User"
                };

                HttpResponseMessage response = await _httpClient.PostAsJsonAsync(
                    "http://localhost:5029/server/auth/register",
                    registerRequest
                );

                //string result = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return "Registration successfull!";
                }
                else
                {
                    string error = await response.Content.ReadAsStringAsync();
                    return $"Error: {response.StatusCode} - {error}";
                }
            }
            catch (HttpRequestException ex)
            {
                return $"Network error: {ex.Message}";
            }
            catch (Exception e)
            {
                return $"Exception accured: {e.Message}";
            }
            
        }
    }
}
