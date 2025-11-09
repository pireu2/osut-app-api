using Microsoft.EntityFrameworkCore;
using OsutApp.Api.Data;
using OsutApp.Api.Models;

namespace OsutApp.Api.Repositories;

public class BoardMemberRepository(ApplicationDbContext context) : IBoardMemberRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<BoardMember?> GetByIdAsync(Guid id)
    {
        return await _context.BoardMembers.FindAsync(id);
    }

    public async Task<BoardMember?> GetByPositionAsync(BoardPosition position)
    {
        return await _context.BoardMembers
            .FirstOrDefaultAsync(bm => bm.Position == position);
    }

    public async Task<BoardMember?> GetByUserIdAsync(string userId)
    {
        return await _context.BoardMembers
            .FirstOrDefaultAsync(bm => bm.UserId == userId);
    }

    public async Task<IEnumerable<BoardMember>> GetAllAsync()
    {
        return await _context.BoardMembers.ToListAsync();
    }

    public async Task AddAsync(BoardMember boardMember)
    {
        await _context.BoardMembers.AddAsync(boardMember);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(BoardMember boardMember)
    {
        _context.BoardMembers.Update(boardMember);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(BoardMember boardMember)
    {
        _context.BoardMembers.Remove(boardMember);
        await _context.SaveChangesAsync();
    }
}
