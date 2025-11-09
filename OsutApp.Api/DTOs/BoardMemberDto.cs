namespace OsutApp.Api.DTOs;

public class BoardMemberDto
{
    public Guid? Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public UserDto? User { get; set; }
    public string Position { get; set; } = string.Empty;
    public DateTime AssignedDate { get; set; }
}