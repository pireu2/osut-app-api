using OsutApp.Api.Models;

namespace OsutApp.Api.Services;

public interface IEventService
{
    Task<IEnumerable<Event>> GetAllEventsAsync();
    Task<IEnumerable<Event>> GetUpcomingEventsAsync();
    Task<Event?> GetEventByIdAsync(Guid id);
    Task<IEnumerable<Event>> GetEventsByDepartmentAsync(Guid departmentId);
    Task<Event> CreateEventAsync(string title, string? description, DateTime dateTime, string location, Guid departmentId);
    Task<Event?> UpdateEventAsync(Guid id, string? title, string? description, DateTime? dateTime, string? location);
    Task<bool> DeleteEventAsync(Guid id);
    Task<bool> SignupForEventAsync(Guid eventId, string userId);
    Task<bool> CancelSignupAsync(Guid eventId, string userId);
    Task<IEnumerable<EventSignup>> GetEventSignupsAsync(Guid eventId);
    Task<IEnumerable<EventSignup>> GetUserSignupsAsync(string userId);
}
