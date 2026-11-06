using Microsoft.AspNetCore.Components.Authorization;
using Blazored.LocalStorage;
using System.Security.Claims;
using System.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;

public class AuthStateProvider : AuthenticationStateProvider
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;
    private readonly AuthenticationState _anonymous;
    public AuthStateProvider(HttpClient httpClient, ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
        _anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");

        if (string.IsNullOrWhiteSpace(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
            return _anonymous;
        }

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);

        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(JwtParser.ParseClaimsFromJwt(token), "jwtAuthType")));
    }

    // use this method as soon as the user logs in
    public async void NotifyUserAuthentication(string token)
    {
        //saves the token in the local storage
        await _localStorage.SetItemAsync("authToken", token);

        var claims = JwtParser.ParseClaimsFromJwt(token);

        //sets the default authorization request header
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var identity = new ClaimsIdentity(
            claims,
            authenticationType: "jwtAuthType",
            nameType: JwtRegisteredClaimNames.Name, 
            roleType: "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
        );

        var authenticatedUser = new ClaimsPrincipal(identity);
        var authState = Task.FromResult(new AuthenticationState(authenticatedUser));

        NotifyAuthenticationStateChanged(authState);
    }

    //  as soon as the user logs out, remove the token from the local storage an the default request header
    public async void NotifyUserLogout()
    {
        await _localStorage.RemoveItemAsync("authToken");

        var authState = Task.FromResult(_anonymous);

        _httpClient.DefaultRequestHeaders.Authorization = null;

        NotifyAuthenticationStateChanged(authState);
    }
}