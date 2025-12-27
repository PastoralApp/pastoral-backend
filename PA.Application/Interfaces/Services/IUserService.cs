using PA.Application.DTOs;

namespace PA.Application.Interfaces.Services;

public interface IUserService
{
    Task<UserDto?> GetByIdAsync(Guid id);
    Task<UserDto?> GetByEmailAsync(string email);
    Task<IEnumerable<UserDto>> GetAllAsync();
    Task<IEnumerable<UserDto>> GetByGrupoAsync(Guid grupoId);
    Task<UserDto> CreateAsync(CreateUserDto dto);
    Task UpdateAsync(Guid id, UpdateUserDto dto);
    Task DeleteAsync(Guid id);
    
    Task<UserProfileDto> GetProfileAsync(Guid userId);
    Task<IEnumerable<UserSelectDto>> GetUsersForSelectAsync(Guid? grupoId = null, Guid? pastoralId = null);
    Task<UserSearchResultDto> SearchUsersAsync(UserSearchDto searchDto);
}
