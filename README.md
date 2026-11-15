# SkillSnap

SkillSnap is a portfolio app created as part of the Microsoft Full-Stack Developer Certification. Create an account, build a profile, and showcase your bio, skills, and projects. Browse profiles to discover what other members are working on.

## Features

- Register and log in with an account
- Create and update a profile with a bio and profile picture
- Add and remove skills and projects
- Browse member profiles
- JWT-based authentication and role-based authorization

## Technologies

- .NET 10
- Blazor WebAssembly
- ASP.NET Core Web API
- ASP.NET Core Identity
- Entity Framework Core
- SQLite
- Telerik UI for Blazor
- Blazored LocalStorage

## How to run the application on your local machine

### 1. Prerequisites

- .NET 10 SDK
- The `dotnet-ef` tool for applying database migrations

If you do not have the EF Core CLI tool installed:

```bash
dotnet tool install --global dotnet-ef
```

### 2. Clone the repository

```bash
git clone https://github.com/msass89/SkillSnap.git
cd SkillSnap
```

### 3. Restore packages

```bash
dotnet restore SkillSnap.Server/SkillSnap.Server.csproj
dotnet restore SkillSnap.Client/SkillSnap.Client.csproj
```

### 4. Configure the application

The server reads JWT settings from configuration. Set these environment variables before starting it:

```text
Jwt__Issuer
Jwt__Audience
Jwt__Key
```

Use a private signing key of at least 256 bits and do not commit production secrets. For local development, set environment variables in your shell or use .NET user secrets.

The server uses SQLite and creates `skillsnap.db` in its working directory. The connection is currently configured in `SkillSnap.Server/Program.cs`.

### 5. Apply database migrations

Run this from the repository root:

```bash
dotnet ef database update --project SkillSnap.Server/SkillSnap.Server.csproj --startup-project SkillSnap.Server/SkillSnap.Server.csproj
```

### 6. Start the application

Start the API and client in separate terminals from the repository root. The HTTP launch profiles use the ports expected by the local client and API configuration.

```bash
dotnet run --project SkillSnap.Server/SkillSnap.Server.csproj --launch-profile http
```

```bash
dotnet run --project SkillSnap.Client/SkillSnap.Client.csproj --launch-profile http
```

Open <http://localhost:5152> in your browser.

## Authentication and authorization

Register from the application, then log in. After registration, the app signs you in and opens your profile. The API issues a JWT on login; the client stores it in local storage and sends it with authenticated requests. Users can update their own profile, skills, and projects.

## Project structure

```text
SkillSnap.Client/
  Components/       Blazor pages, layouts, and UI components
  Services/         API client services
  AuthProviders/    Authentication state and JWT handling
  wwwroot/          Client static assets

SkillSnap.Server/
  Controllers/      Authentication, profile, skill, and project API
  Data/             EF Core context and role seeding
  Migrations/       Entity Framework Core migrations
  Models/           Server-side entities

SkillSnap.Shared/
  DTOs/             Data transfer objects shared by client and server
```

## License

MIT License. Copyright (c) 2026 Maike Saß.
