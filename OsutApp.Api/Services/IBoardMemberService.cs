using OsutApp.Api.DTOs;
using OsutApp.Api.Models;

namespace OsutApp.Api.Services;

public interface IBoardMemberService
{
    Task<IEnumerable<BoardMemberDto>> GetAllBoardMembersAsync();
    Task<BoardMemberDto?> GetBoardMemberByIdAsync(Guid id);
    Task<BoardMemberDto?> GetBoardMemberByPositionAsync(BoardPosition position);
    Task<BoardMemberDto?> GetBoardMemberByUserIdAsync(string userId);
    Task<BoardMemberDto> AssignBoardMemberAsync(BoardMemberDto boardMemberDto);
    Task<BoardMemberDto?> UpdateBoardMemberPositionAsync(Guid id, BoardMemberDto boardMemberDto);
    Task<bool> RemoveBoardMemberAsync(Guid id);
}
