using OsutApp.Api.Models;
using OsutApp.Api.Repositories;

namespace OsutApp.Api.Services;

public interface IBoardMemberService
{
    Task<IEnumerable<BoardMember>> GetAllBoardMembersAsync();
    Task<BoardMember?> GetBoardMemberByIdAsync(Guid id);
    Task<BoardMember?> GetBoardMemberByPositionAsync(BoardPosition position);
    Task<BoardMember?> GetBoardMemberByUserIdAsync(string userId);
    Task<BoardMember> AssignBoardMemberAsync(string userId, BoardPosition position);
    Task<BoardMember?> UpdateBoardMemberPositionAsync(Guid id, BoardPosition newPosition);
    Task<bool> RemoveBoardMemberAsync(Guid id);
}

public class BoardMemberService : IBoardMemberService
{
    private readonly IBoardMemberRepository _boardMemberRepository;
    private readonly IUserRepository _userRepository;

    public BoardMemberService(IBoardMemberRepository boardMemberRepository, IUserRepository userRepository)
    {
        _boardMemberRepository = boardMemberRepository;
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<BoardMember>> GetAllBoardMembersAsync()
    {
        return await _boardMemberRepository.GetAllAsync();
    }

    public async Task<BoardMember?> GetBoardMemberByIdAsync(Guid id)
    {
        return await _boardMemberRepository.GetByIdAsync(id);
    }

    public async Task<BoardMember?> GetBoardMemberByPositionAsync(BoardPosition position)
    {
        return await _boardMemberRepository.GetByPositionAsync(position);
    }

    public async Task<BoardMember?> GetBoardMemberByUserIdAsync(string userId)
    {
        return await _boardMemberRepository.GetByUserIdAsync(userId);
    }

    public async Task<BoardMember> AssignBoardMemberAsync(string userId, BoardPosition position)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new ArgumentException("Invalid user ID");
        }

        var existingMember = await _boardMemberRepository.GetByPositionAsync(position);
        if (existingMember != null)
        {
            throw new InvalidOperationException($"Position {position} is already assigned to another member");
        }

        var userBoardMember = await _boardMemberRepository.GetByUserIdAsync(userId);
        if (userBoardMember != null)
        {
            throw new InvalidOperationException("User is already assigned to a board position");
        }

        var boardMember = new BoardMember
        {
            UserId = userId,
            Position = position
        };

        await _boardMemberRepository.AddAsync(boardMember);
        return boardMember;
    }

    public async Task<BoardMember?> UpdateBoardMemberPositionAsync(Guid id, BoardPosition newPosition)
    {
        var boardMember = await _boardMemberRepository.GetByIdAsync(id);
        if (boardMember == null)
        {
            return null;
        }

        // Check if new position is already taken
        if (boardMember.Position != newPosition)
        {
            var existingMember = await _boardMemberRepository.GetByPositionAsync(newPosition);
            if (existingMember != null)
            {
                throw new InvalidOperationException($"Position {newPosition} is already assigned to another member");
            }
        }

        boardMember.Position = newPosition;
        await _boardMemberRepository.UpdateAsync(boardMember);
        return boardMember;
    }

    public async Task<bool> RemoveBoardMemberAsync(Guid id)
    {
        var boardMember = await _boardMemberRepository.GetByIdAsync(id);
        if (boardMember == null)
        {
            return false;
        }

        await _boardMemberRepository.DeleteAsync(boardMember);
        return true;
    }
}