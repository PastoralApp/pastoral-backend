using PA.Application.DTOs;

namespace PA.Application.Interfaces.Services;

public interface IJogoService
{
    Task<JogoDto?> GetByIdAsync(Guid id);
    Task<JogoDto?> GetByIdWithDetailsAsync(Guid id);
    Task<IEnumerable<JogoDto>> GetAllAsync();
    Task<JogoDto> CreateAsync(CreateJogoDto dto);
    Task<JogoDto> UpdateAsync(Guid id, UpdateJogoDto dto);
    Task DeleteAsync(Guid id);
}
