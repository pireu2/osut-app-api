using OsutApp.Api.Models;

namespace OsutApp.Api.Services;

public interface IAuthService
{
    Task<AuthResult> LoginAsync(string idToken);
    Task<AuthResult> RefreshAsync(string refreshToken);
    Task<bool> LogoutAsync(string refreshToken);
}

public class AuthResult
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}