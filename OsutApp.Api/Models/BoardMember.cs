using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OsutApp.Api.Models;

public class BoardMember
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public required string UserId { get; set; }

    [ForeignKey("UserId")]
    public User? User { get; set; }

    [Required]
    public required BoardPosition Position { get; set; }

    [Required]
    public DateTime AssignedDate { get; set; } = DateTime.UtcNow;
}