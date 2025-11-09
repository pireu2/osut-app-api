using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OsutApp.Api.Models;

public class Department
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public required string Name { get; set; }

    public string? Description { get; set; }

    [Required]
    public required DepartmentType Type { get; set; }

    [Required]
    public required string CoordinatorId { get; set; }

    [ForeignKey("CoordinatorId")]
    public User? Coordinator { get; set; }

    public ICollection<Event> Events { get; set; } = [];
}