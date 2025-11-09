using OsutApp.Api.Models;

namespace OsutApp.Api.Repositories;

public interface IEventSignupRepository
{
    Task<EventSignup?> GetByIdAsync(Guid id);
    Task<IEnumerable<EventSignup>> GetByEventAsync(Guid eventId);
    Task<IEnumerable<EventSignup>> GetByUserAsync(string userId);
    Task<EventSignup?> GetByEventAndUserAsync(Guid eventId, string userId);
    Task AddAsync(EventSignup signup);
    Task UpdateAsync(EventSignup signup);
    Task DeleteAsync(EventSignup signup);
}