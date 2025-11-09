using System.ComponentModel.DataAnnotations;

namespace OsutApp.Api.Models;

public class MemberWhitelist
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    [Required]
    public required UserRole Role { get; set; }

    public bool IsActive { get; set; } = true;
}