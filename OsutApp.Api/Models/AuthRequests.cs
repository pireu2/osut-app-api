namespace OsutApp.Api.Models;

public class LoginRequest
{
    public required string IdToken { get; set; }
}

public class RefreshRequest
{
    public required string RefreshToken { get; set; }
}

public class LogoutRequest
{
    public required string RefreshToken { get; set; }
}