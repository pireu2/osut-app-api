using OsutApp.Api.Models;

namespace OsutApp.Api.Repositories;

public interface IEventRepository
{
    Task<Event?> GetByIdAsync(Guid id);
    Task<IEnumerable<Event>> GetAllAsync();
    Task<IEnumerable<Event>> GetByDepartmentAsync(Guid departmentId);
    Task<IEnumerable<Event>> GetUpcomingAsync();
    Task AddAsync(Event eventEntity);
    Task UpdateAsync(Event eventEntity);
    Task DeleteAsync(Event eventEntity);
}