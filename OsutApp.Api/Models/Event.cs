using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OsutApp.Api.Models;

public class Event
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public required string Title { get; set; }

    public string? Description { get; set; }

    [Required]
    public required DateTime DateTime { get; set; }

    [Required]
    public required string Location { get; set; }

    [Required]
    public required Guid DepartmentId { get; set; }

    [ForeignKey("DepartmentId")]
    public Department? Department { get; set; }

    public ICollection<EventSignup> Signups { get; set; } = [];
}