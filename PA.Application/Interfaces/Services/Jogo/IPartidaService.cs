using PA.Application.DTOs;

namespace PA.Application.Interfaces.Services;

public interface IPartidaService
{
    Task<PartidaDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<PartidaDto>> GetAllAsync();
    Task<IEnumerable<PartidaDto>> GetByModalidadeIdAsync(Guid modalidadeId);
    Task<IEnumerable<PartidaDto>> GetByGrupoIdAsync(Guid grupoId);
    Task<IEnumerable<PartidaDto>> GetPendentesAsync();
    Task<PartidaDto> CreateAsync(CreatePartidaDto dto);
    Task<PartidaDto> RegistrarResultadoAsync(Guid partidaId, RegistrarResultadoPartidaDto dto);
    Task DeleteAsync(Guid id);
}
