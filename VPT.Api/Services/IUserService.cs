using VPT.Api.DTOs;

namespace VPT.Api.Services;

public interface IUserService
{
    Task<UserDto?> CreateAsync(CreateUserDto dto);

    Task<UserDto?> LoginAsync(LoginDto dto);

    Task<UserDto?> GetByIdAsync(int id);

    Task<List<UserDto>> GetAllAsync();

    Task<bool> UpdateAsync(int id, CreateUserDto dto);

    Task<bool> DeleteAsync(int id);
}