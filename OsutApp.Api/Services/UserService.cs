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
        var existingUser = await _userRepository.GetByIdAsync(userDto.Id!);

        if (existingUser == null)
        {
            throw new ArgumentException("User not found");
        }

        if (!string.IsNullOrEmpty(userDto.FirstName))
        {
            existingUser.FirstName = userDto.FirstName;
        }

        if (!string.IsNullOrEmpty(userDto.LastName))
        {
            existingUser.LastName = userDto.LastName;
        }

        if (userDto.YearOfBirth.HasValue)
        {
            existingUser.YearOfBirth = userDto.YearOfBirth;
        }

        if (!string.IsNullOrEmpty(userDto.ProfilePictureUrl))
        {
            existingUser.ProfilePictureUrl = userDto.ProfilePictureUrl;
        }

        if (!string.IsNullOrEmpty(userDto.Status))
        {
            existingUser.Status = Enum.Parse<VolunteerStatus>(userDto.Status);
        }

        existingUser.IsAdmin = userDto.IsAdmin;

        if (!string.IsNullOrEmpty(userDto.Email))
        {
            existingUser.Email = userDto.Email;
        }

        if (!string.IsNullOrEmpty(userDto.UserName))
        {
            existingUser.UserName = userDto.UserName;
        }

        await _userRepository.UpdateAsync(existingUser);
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