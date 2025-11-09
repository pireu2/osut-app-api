namespace OsutApp.Api.DTOs;

public class DepartmentDto
{
    public Guid? Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Type { get; set; } = string.Empty;
    public string CoordinatorId { get; set; } = string.Empty;
    public UserDto? Coordinator { get; set; }
    public int EventsCount { get; set; }
}