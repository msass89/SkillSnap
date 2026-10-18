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
            return _anonymous;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(JwtParser.ParseClaimsFromJwt(token), "jwtAuthType")));
    }

    // use this method as soon as the user logs in
    public void NotifyUserAuthentication(string token)
    {
        var claims = JwtParser.ParseClaimsFromJwt(token);

        var identity = new ClaimsIdentity(
            claims,
            authenticationType: "jwtAuthType",
            nameType: JwtRegisteredClaimNames.Name, 
            roleType: "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
        );

        var authenticatedUser = new ClaimsPrincipal(identity);
        var authState = Task.FromResult(new AuthenticationState(authenticatedUser));

        /*foreach (var claim in claims)
        {
            Console.WriteLine($"{claim.Type} = {claim.Value}");
        }*/

        //Console.WriteLine($"Identity.Name = {identity.Name}");
        NotifyAuthenticationStateChanged(authState);
    }

    //  as soon as the user logs out
    public void NotifyUserLogout()
    {
        var authState = Task.FromResult(_anonymous);
        NotifyAuthenticationStateChanged(authState);
    }
}