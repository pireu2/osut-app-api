using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OsutApp.Api.DTOs;
using OsutApp.Api.Models;
using OsutApp.Api.Services;

namespace OsutApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BoardMembersController(
    IBoardMemberService boardMemberService) : ControllerBase
{
    private readonly IBoardMemberService _boardMemberService = boardMemberService;

    [HttpGet]
    public async Task<IActionResult> GetAllBoardMembers()
    {
        var boardMembers = await _boardMemberService.GetAllBoardMembersAsync();

        return Ok(boardMembers);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBoardMember(Guid id)
    {
        var boardMember = await _boardMemberService.GetBoardMemberByIdAsync(id);

        if (boardMember == null)
        {
            return NotFound();
        }

        return Ok(boardMember);
    }

    [HttpGet("position/{position}")]
    public async Task<IActionResult> GetBoardMemberByPosition(BoardPosition position)
    {
        var boardMember = await _boardMemberService.GetBoardMemberByPositionAsync(position);

        if (boardMember == null)
        {
            return NotFound();
        }

        return Ok(boardMember);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetBoardMemberByUser(string userId)
    {
        var boardMember = await _boardMemberService.GetBoardMemberByUserIdAsync(userId);

        if (boardMember == null)
        {
            return NotFound();
        }

        return Ok(boardMember);
    }

    [HttpPost]
    public async Task<IActionResult> AssignBoardMember([FromBody] BoardMemberDto request)
    {
        try
        {
            var boardMember = await _boardMemberService.AssignBoardMemberAsync(request);

            return CreatedAtAction(nameof(GetBoardMember), new { id = boardMember.Id }, boardMember);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBoardMemberPosition(Guid id, [FromBody] BoardMemberDto request)
    {
        try
        {
            var boardMember = await _boardMemberService.UpdateBoardMemberPositionAsync(id, request);

            if (boardMember == null)
            {
                return NotFound();
            }

            return Ok(boardMember);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveBoardMember(Guid id)
    {
        var success = await _boardMemberService.RemoveBoardMemberAsync(id);

        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }
}