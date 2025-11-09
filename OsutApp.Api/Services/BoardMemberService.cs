using AutoMapper;
using OsutApp.Api.DTOs;
using OsutApp.Api.Models;
using OsutApp.Api.Repositories;

namespace OsutApp.Api.Services;

public class BoardMemberService : IBoardMemberService
{
    private readonly IBoardMemberRepository _boardMemberRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public BoardMemberService(IBoardMemberRepository boardMemberRepository, IUserRepository userRepository, IMapper mapper)
    {
        _boardMemberRepository = boardMemberRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<BoardMemberDto>> GetAllBoardMembersAsync()
    {
        var boardMembers = await _boardMemberRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<BoardMemberDto>>(boardMembers);
    }

    public async Task<BoardMemberDto?> GetBoardMemberByIdAsync(Guid id)
    {
        var boardMember = await _boardMemberRepository.GetByIdAsync(id);
        return boardMember != null ? _mapper.Map<BoardMemberDto>(boardMember) : null;
    }

    public async Task<BoardMemberDto?> GetBoardMemberByPositionAsync(BoardPosition position)
    {
        var boardMember = await _boardMemberRepository.GetByPositionAsync(position);
        return boardMember != null ? _mapper.Map<BoardMemberDto>(boardMember) : null;
    }

    public async Task<BoardMemberDto?> GetBoardMemberByUserIdAsync(string userId)
    {
        var boardMember = await _boardMemberRepository.GetByUserIdAsync(userId);
        return boardMember != null ? _mapper.Map<BoardMemberDto>(boardMember) : null;
    }

    public async Task<BoardMemberDto> AssignBoardMemberAsync(BoardMemberDto boardMemberDto)
    {
        var user = await _userRepository.GetByIdAsync(boardMemberDto.UserId);

        if (user == null)
        {
            throw new ArgumentException("Invalid user ID");
        }

        var position = Enum.Parse<BoardPosition>(boardMemberDto.Position);
        var existingMember = await _boardMemberRepository.GetByPositionAsync(position);

        if (existingMember != null)
        {
            throw new InvalidOperationException($"Position {position} is already assigned to another member");
        }

        var userBoardMember = await _boardMemberRepository.GetByUserIdAsync(boardMemberDto.UserId);

        if (userBoardMember != null)
        {
            throw new InvalidOperationException("User is already assigned to a board position");
        }

        var boardMember = new BoardMember
        {
            UserId = boardMemberDto.UserId,
            Position = position
        };

        await _boardMemberRepository.AddAsync(boardMember);

        return _mapper.Map<BoardMemberDto>(boardMember);
    }

    public async Task<BoardMemberDto?> UpdateBoardMemberPositionAsync(Guid id, BoardMemberDto boardMemberDto)
    {
        var boardMember = await _boardMemberRepository.GetByIdAsync(id);
        if (boardMember == null)
        {
            return null;
        }

        var newPosition = Enum.Parse<BoardPosition>(boardMemberDto.Position);

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

        return _mapper.Map<BoardMemberDto>(boardMember);
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