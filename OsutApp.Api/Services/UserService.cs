using AutoMapper;
using OsutApp.Api.DTOs;
using OsutApp.Api.Models;
using OsutApp.Api.Repositories;

namespace OsutApp.Api.Services;

public class UserService(IUserRepository userRepository, IMapper mapper) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<UserDto?> GetUserByIdAsync(string id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        return user != null ? _mapper.Map<UserDto>(user) : null;
    }

    public async Task<UserDto?> GetUserByEmailAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);

        return user != null ? _mapper.Map<UserDto>(user) : null;
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();

        return _mapper.Map<IEnumerable<UserDto>>(users);
    }

    public async Task UpdateUserAsync(UserDto userDto)
    {
        var user = _mapper.Map<User>(userDto);

        await _userRepository.UpdateAsync(user);
    }

    public async Task DeleteUserAsync(string id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user != null)
        {
            await _userRepository.DeleteAsync(user);
        }
    }

    public async Task<bool> IsUserAdminAsync(string userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        return user?.IsAdmin ?? false;
    }
}