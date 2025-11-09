using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OsutApp.Api.DTOs;
using OsutApp.Api.Services;
using System.Security.Claims;

namespace OsutApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController(IUserService userService) : BaseController
{
    private readonly IUserService _userService = userService;

    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();

        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(string id)
    {
        var user = await _userService.GetUserByIdAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] UserDto userDto)
    {
        if (id != userDto.Id)
        {
            return BadRequest();
        }

        try
        {
            var currentUserId = GetCurrentUserId();
            var isAdmin = await _userService.IsUserAdminAsync(currentUserId.ToString());
            var isOwner = currentUserId.ToString() == id;

            if (!isAdmin && !isOwner)
            {
                return Forbid("Only administrators or the account owner can update users");
            }

            await _userService.UpdateUserAsync(userDto);

            return NoContent();
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteUser(string id)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var isAdmin = await _userService.IsUserAdminAsync(currentUserId.ToString());
            var isOwner = currentUserId.ToString() == id;

            if (!isAdmin && !isOwner)
            {
                return Forbid("Only administrators or the account owner can delete users");
            }

            await _userService.DeleteUserAsync(id);

            return NoContent();
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }
}