using PA.Application.DTOs;

namespace PA.Application.Interfaces.Services;

public interface ITrofeuService
{
    Task<TrofeuDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<TrofeuDto>> GetAllAsync();
    Task<IEnumerable<TrofeuDto>> GetByJogoIdAsync(Guid jogoId);
    Task<IEnumerable<TrofeuDto>> GetByGrupoIdAsync(Guid grupoId);
    Task<IEnumerable<TrofeuDto>> GetByAnoAsync(int ano);
    Task<TrofeuDto> CreateAsync(CreateTrofeuDto dto);
    Task DeleteAsync(Guid id);
}
