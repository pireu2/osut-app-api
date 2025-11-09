namespace OsutApp.Api.DTOs;

public class EventSignupDto
{
    public Guid? Id { get; set; }
    public Guid EventId { get; set; }
    public EventDto? Event { get; set; }
    public string UserId { get; set; } = string.Empty;
    public UserDto? User { get; set; }
    public DateTime SignupDate { get; set; }
}