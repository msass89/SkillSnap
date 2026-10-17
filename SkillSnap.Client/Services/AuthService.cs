using System.Net.Http.Json;
using SkillSnap.Shared;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.LocalStorage;
using System.Text.Json;
using System.Net.Http.Headers;
using System.Text;

namespace SkillSnap.Client.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options;
        private readonly AuthenticationStateProvider _authStateProvider; 
        private readonly ILocalStorageService _localStorage;

        public AuthService(HttpClient httpClient, AuthenticationStateProvider authStateProvider, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            _authStateProvider = authStateProvider; 
            _localStorage = localStorage;
        }

        public async Task<bool> CreateUser(string fullName, string email, string password)
        {
            try
            {
                RegisterRequestDto registerRequest = new RegisterRequestDto
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

        public async Task<TokenResponseDto> LoginAsync(LoginRequestDto loginRequest)
        {
            try
            {
                // send the request to the login endpoint and deserialize the response
                var content = JsonSerializer.Serialize(loginRequest);
                var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
                var authResult = await _httpClient.PostAsync("http://localhost:5029/server/auth/login", bodyContent);
                
                var authContent = await authResult.Content.ReadAsStringAsync();
                
                var result = JsonSerializer.Deserialize<TokenResponseDto>(authContent, _options);
                
                //if it is not successful, return that response containing the error message
                if (!authResult.IsSuccessStatusCode)
                    return new TokenResponseDto { IsAuthSuccessful = false, ErrorMessage = "Login was not successful" };
                
                Console.WriteLine("Token result: " + result.Token);

                // otherwise set the token in the local storage
                await _localStorage.SetItemAsync("authToken", result.Token);

                ((AuthStateProvider)_authStateProvider).NotifyUserAuthentication(result.Token);
                
                // add the token as a default authorization header for the HttpClient and return the successful result
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", result.Token);
                
                return new TokenResponseDto { IsAuthSuccessful = true };
            }
            catch (Exception e)
            {                
                return new TokenResponseDto { IsAuthSuccessful = false, ErrorMessage = e.Message };
            }
        }

        public async Task Logout()
        {
            // remove the token from the local storage and from the default authorization header
            await _localStorage.RemoveItemAsync("authToken");
            ((AuthStateProvider)_authStateProvider).NotifyUserLogout();
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }

    }
}
