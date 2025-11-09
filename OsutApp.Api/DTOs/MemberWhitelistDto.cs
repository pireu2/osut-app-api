namespace OsutApp.Api.DTOs;

public class MemberWhitelistDto
{
    public Guid? Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}