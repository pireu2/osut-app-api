namespace OsutApp.Api.DTOs;

public class UserDto
{
    public string? Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public int? YearOfBirth { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string Status { get; set; } = "Recruit";
    public bool IsAdmin { get; set; }
    public string? Email { get; set; }
    public string? UserName { get; set; }
}