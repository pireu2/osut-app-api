using Microsoft.EntityFrameworkCore;
using OsutApp.Api.Data;
using OsutApp.Api.Models;

namespace OsutApp.Api.Repositories;

public class MemberWhitelistRepository(ApplicationDbContext context) : IMemberWhitelistRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<MemberWhitelist?> GetActiveByEmailAsync(string email)
    {
        return await _context.MemberWhitelists
            .FirstOrDefaultAsync(w => w.Email == email && w.IsActive);
    }

    public async Task<IEnumerable<MemberWhitelist>> GetAllAsync()
    {
        return await _context.MemberWhitelists.ToListAsync();
    }

    public async Task AddAsync(MemberWhitelist whitelist)
    {
        await _context.MemberWhitelists.AddAsync(whitelist);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(MemberWhitelist whitelist)
    {
        _context.MemberWhitelists.Update(whitelist);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(MemberWhitelist whitelist)
    {
        _context.MemberWhitelists.Remove(whitelist);
        await _context.SaveChangesAsync();
    }
}