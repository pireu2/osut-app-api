using OsutApp.Api.DTOs;
using OsutApp.Api.Models;

namespace OsutApp.Api.Services;

public interface IEventService
{
    Task<IEnumerable<EventDto>> GetAllEventsAsync();
    Task<IEnumerable<EventDto>> GetUpcomingEventsAsync();
    Task<EventDto?> GetEventByIdAsync(Guid id);
    Task<IEnumerable<EventDto>> GetEventsByDepartmentAsync(Guid departmentId);
    Task<EventDto> CreateEventAsync(EventDto eventDto);
    Task<EventDto?> UpdateEventAsync(Guid id, EventDto eventDto);
    Task<bool> DeleteEventAsync(Guid id);
    Task<bool> SignupForEventAsync(Guid eventId, string userId);
    Task<bool> CancelSignupAsync(Guid eventId, string userId);
    Task<IEnumerable<EventSignupDto>> GetEventSignupsAsync(Guid eventId);
    Task<IEnumerable<EventSignupDto>> GetUserSignupsAsync(string userId);
}
