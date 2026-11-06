using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SkillSnap.Client;
using SkillSnap.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.LocalStorage;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// sets the base adress to the Api-port
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5029/") });

builder.Services.AddScoped<ProjectServices>();
builder.Services.AddScoped<SkillService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UserService>();

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore(); //adds authorization service


builder.Services.AddScoped<AuthStateProvider>();

builder.Services.AddScoped<AuthenticationStateProvider>(
    sp => sp.GetRequiredService<AuthStateProvider>());

await builder.Build().RunAsync();
