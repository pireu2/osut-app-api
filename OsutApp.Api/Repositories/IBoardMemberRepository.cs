using OsutApp.Api.Models;

namespace OsutApp.Api.Repositories;

public interface IBoardMemberRepository
{
    Task<BoardMember?> GetByIdAsync(Guid id);
    Task<BoardMember?> GetByPositionAsync(BoardPosition position);
    Task<BoardMember?> GetByUserIdAsync(string userId);
    Task<IEnumerable<BoardMember>> GetAllAsync();
    Task AddAsync(BoardMember boardMember);
    Task UpdateAsync(BoardMember boardMember);
    Task DeleteAsync(BoardMember boardMember);
}