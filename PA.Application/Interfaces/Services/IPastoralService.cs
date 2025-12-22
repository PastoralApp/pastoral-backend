using PA.Application.DTOs;

namespace PA.Application.Interfaces.Services;

public interface IPastoralService
{
    Task<PastoralDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<PastoralDto>> GetAllAsync(bool incluirInativas = false);
    Task<PastoralDto> CreateAsync(CreatePastoralDto dto);
    Task UpdateAsync(Guid id, CreatePastoralDto dto);
    Task DeleteAsync(Guid id);
    Task DesativarAsync(Guid id);
    Task AtivarAsync(Guid id);
}
