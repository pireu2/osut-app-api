# OsutApp API

A Student Organization Volunteer App backend built with ASP.NET Core.

## Setup

1. Clone the repository
2. Copy `OsutApp.Api/appsettings.json.template` to `OsutApp.Api/appsettings.json`
3. Fill in your actual configuration values in `appsettings.json`

### Configuration

The application requires the following configuration values:

- **Database Connection**: PostgreSQL connection string
- **JWT Settings**: Secret key for token signing (generate with `openssl rand -base64 32`)

### Development

For development, you can use User Secrets instead of modifying `appsettings.json`:

```bash
cd OsutApp.Api
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "your-connection-string"
dotnet user-secrets set "Jwt:Key" "your-jwt-key"
```

### Database

Run migrations to set up the database:

```bash
dotnet ef database update
```

### Running the Application

```bash
dotnet run
```

The API will be available at `https://localhost:7202` (or the port configured in launchSettings.json).
