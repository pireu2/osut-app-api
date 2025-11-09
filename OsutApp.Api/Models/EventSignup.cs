using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OsutApp.Api.Models;

public class EventSignup
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public required Guid EventId { get; set; }

    [ForeignKey("EventId")]
    public Event? Event { get; set; }

    [Required]
    public required string UserId { get; set; }

    [ForeignKey("UserId")]
    public User? User { get; set; }

    [Required]
    public DateTime SignupDate { get; set; } = DateTime.UtcNow;

    public EventSignupStatus Status { get; set; } = EventSignupStatus.Pending;
}

public enum EventSignupStatus
{
    Pending,
    Confirmed,
    Cancelled
}