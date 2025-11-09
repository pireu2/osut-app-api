using Google.Apis.Auth;
using Microsoft.IdentityModel.Tokens;
using OsutApp.Api.Models;
using OsutApp.Api.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OsutApp.Api.Services;

public class AuthService(
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IMemberWhitelistRepository memberWhitelistRepository,
    IConfiguration configuration) : IAuthService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;
    private readonly IMemberWhitelistRepository _memberWhitelistRepository = memberWhitelistRepository;
    private readonly IConfiguration _configuration = configuration;

    public async Task<AuthResult> LoginAsync(string idToken)
    {
        var payload = await GoogleJsonWebSignature.ValidateAsync(idToken);

        var whitelistEntry = await _memberWhitelistRepository.GetActiveByEmailAsync(payload.Email);

        if (whitelistEntry == null)
        {
            return new AuthResult { Success = false, ErrorMessage = "Email not authorized" };
        }

        var user = await _userRepository.GetByEmailAsync(payload.Email);
        if (user == null)
        {
            user = new User
            {
                UserName = payload.Email,
                Email = payload.Email,
                FirstName = payload.GivenName,
                LastName = payload.FamilyName,
                ProfilePictureUrl = payload.Picture,
                IsAdmin = whitelistEntry.Role == UserRole.Admin
            };
            await _userRepository.AddAsync(user);
        }

        var roleClaim = user.IsAdmin ? "Admin" : "Volunteer";
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(ClaimTypes.Role, roleClaim)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(15),
            signingCredentials: creds);

        var refreshToken = Guid.NewGuid().ToString("N");
        var refreshTokenEntity = new RefreshToken
        {
            Token = refreshToken,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };
        await _refreshTokenRepository.AddAsync(refreshTokenEntity);

        return new AuthResult
        {
            Success = true,
            AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
            RefreshToken = refreshToken
        };
    }

    public async Task<AuthResult> RefreshAsync(string refreshToken)
    {
        var storedToken = await _refreshTokenRepository.GetValidTokenWithUserAsync(refreshToken);

        if (storedToken == null)
            return new AuthResult { Success = false, ErrorMessage = "Invalid refresh token" };

        var user = storedToken.User!;

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(ClaimTypes.Role, user.IsAdmin ? "Admin" : "Volunteer")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var newAccessToken = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(15),
            signingCredentials: creds);

        // Generate new refresh token
        var newRefreshToken = Guid.NewGuid().ToString("N");
        storedToken.IsRevoked = true;
        await _refreshTokenRepository.UpdateAsync(storedToken);
        var newRefreshEntity = new RefreshToken
        {
            Token = newRefreshToken,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };
        await _refreshTokenRepository.AddAsync(newRefreshEntity);

        return new AuthResult
        {
            Success = true,
            AccessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
            RefreshToken = newRefreshToken
        };
    }

    public async Task<bool> LogoutAsync(string refreshToken)
    {
        var token = await _refreshTokenRepository.GetByTokenAsync(refreshToken);
        if (token != null)
        {
            token.IsRevoked = true;
            await _refreshTokenRepository.UpdateAsync(token);
            return true;
        }
        return false;
    }
}