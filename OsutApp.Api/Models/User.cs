using Microsoft.AspNetCore.Identity;

namespace OsutApp.Api.Models;

public class User : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public int? YearOfBirth { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public VolunteerStatus Status { get; set; } = VolunteerStatus.Recruit;
    public bool IsAdmin { get; set; } = false;
}