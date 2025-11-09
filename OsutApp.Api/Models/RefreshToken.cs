using System.ComponentModel.DataAnnotations;

namespace OsutApp.Api.Models;

public class RefreshToken
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public required string Token { get; set; }

    [Required]
    public required string UserId { get; set; }

    [Required]
    public DateTime ExpiresAt { get; set; }

    public bool IsRevoked { get; set; } = false;

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User? User { get; set; }
}