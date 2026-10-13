using Microsoft.EntityFrameworkCore;
using SkillSnap.Server.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var  MyAllowedOrigin = "_myAllowedOrigin";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();

builder.Services.AddDbContext<SkillSnapContext>(options =>
    options.UseSqlite("Data Source=skillsnap.db"));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<SkillSnapContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllers();

// Configure CORS to allow requests from the Blazor WebAssembly app
builder.Services.AddCors( options =>
{
    options.AddPolicy(name: MyAllowedOrigin,
        policy =>
        {
            policy.WithOrigins("http://localhost:5152")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});


//add JwtBearerAuthentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true, 
            ValidIssuer = builder.Configuration["Jwt:Issuer"], 
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

var app = builder.Build();

// seed the roles into the database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    await IdentitySeed.SeedRolesAsync(services);
}

app.UseCors(MyAllowedOrigin);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
