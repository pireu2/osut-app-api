using Microsoft.EntityFrameworkCore;
using OsutApp.Api.Data;
using OsutApp.Api.Models;

namespace OsutApp.Api.Repositories;

public class EventRepository(ApplicationDbContext context) : IEventRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Event?> GetByIdAsync(Guid id)
    {
        return await _context.Events
            .Include(e => e.Department)
            .Include(e => e.Signups)
                .ThenInclude(s => s.User)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<IEnumerable<Event>> GetAllAsync()
    {
        return await _context.Events
            .Include(e => e.Department)
            .OrderBy(e => e.DateTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<Event>> GetByDepartmentAsync(Guid departmentId)
    {
        return await _context.Events
            .Include(e => e.Department)
            .Where(e => e.DepartmentId == departmentId)
            .OrderBy(e => e.DateTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<Event>> GetUpcomingAsync()
    {
        return await _context.Events
            .Include(e => e.Department)
            .Where(e => e.DateTime > DateTime.UtcNow)
            .OrderBy(e => e.DateTime)
            .ToListAsync();
    }

    public async Task AddAsync(Event eventEntity)
    {
        await _context.Events.AddAsync(eventEntity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Event eventEntity)
    {
        _context.Events.Update(eventEntity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Event eventEntity)
    {
        _context.Events.Remove(eventEntity);
        await _context.SaveChangesAsync();
    }
}