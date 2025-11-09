namespace OsutApp.Api.DTOs;

public class EventDto
{
    public Guid? Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime DateTime { get; set; }
    public string Location { get; set; } = string.Empty;
    public Guid DepartmentId { get; set; }
    public DepartmentDto? Department { get; set; }
    public int SignupsCount { get; set; }
}