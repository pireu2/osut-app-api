using OsutApp.Api.DTOs;
using OsutApp.Api.Models;

namespace OsutApp.Api.Services;

public interface IUserService
{
    Task<UserDto?> GetUserByIdAsync(string id);
    Task<UserDto?> GetUserByEmailAsync(string email);
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task UpdateUserAsync(UserDto userDto);
    Task DeleteUserAsync(string id);
}