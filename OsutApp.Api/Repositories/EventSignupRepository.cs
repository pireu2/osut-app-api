using Microsoft.EntityFrameworkCore;
using OsutApp.Api.Data;
using OsutApp.Api.Models;

namespace OsutApp.Api.Repositories;

public class EventSignupRepository(ApplicationDbContext context) : IEventSignupRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<EventSignup?> GetByIdAsync(Guid id)
    {
        return await _context.EventSignups
            .Include(s => s.Event)
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<IEnumerable<EventSignup>> GetByEventAsync(Guid eventId)
    {
        return await _context.EventSignups
            .Include(s => s.User)
            .Where(s => s.EventId == eventId)
            .OrderBy(s => s.SignupDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<EventSignup>> GetByUserAsync(string userId)
    {
        return await _context.EventSignups
            .Include(s => s.Event)
                .ThenInclude(e => e!.Department)
            .Where(s => s.UserId == userId)
            .OrderBy(s => s.Event!.DateTime)
            .ToListAsync();
    }

    public async Task<EventSignup?> GetByEventAndUserAsync(Guid eventId, string userId)
    {
        return await _context.EventSignups
            .FirstOrDefaultAsync(s => s.EventId == eventId && s.UserId == userId);
    }

    public async Task AddAsync(EventSignup signup)
    {
        await _context.EventSignups.AddAsync(signup);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(EventSignup signup)
    {
        _context.EventSignups.Update(signup);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(EventSignup signup)
    {
        _context.EventSignups.Remove(signup);
        await _context.SaveChangesAsync();
    }
}