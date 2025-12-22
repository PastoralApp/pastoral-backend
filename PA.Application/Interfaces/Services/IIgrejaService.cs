using PA.Application.DTOs;

namespace PA.Application.Interfaces.Services;

public interface IIgrejaService
{
    Task<IgrejaDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<IgrejaDto>> GetAllAsync(bool incluirInativas = false);
    Task<IgrejaDto> CreateAsync(CreateIgrejaDto dto);
    Task UpdateAsync(Guid id, CreateIgrejaDto dto);
    Task DeleteAsync(Guid id);
    Task DesativarAsync(Guid id);
    Task AtivarAsync(Guid id);
}
