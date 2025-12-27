using PA.Application.DTOs;
using PA.Domain.Enums;

namespace PA.Application.Interfaces.Services;

public interface IGuiaService
{
    Task<GuiaDto?> GetByIdAsync(Guid id);
    Task<GuiaDto?> GetByIdWithDetailsAsync(Guid id);
    Task<IEnumerable<GuiaDto>> GetAllAsync();
    Task<IEnumerable<GuiaDto>> GetByPastoralIdAsync(Guid pastoralId);
    Task<IEnumerable<GuiaDto>> GetByAnoAsync(int ano);
    Task<IEnumerable<GuiaDto>> GetByStatusAsync(StatusJogo status);
    Task<GuiaDto> CreateAsync(CreateGuiaDto dto, Guid userId);
    Task<GuiaDto> UpdateAsync(Guid id, UpdateGuiaDto dto, Guid userId);
    Task DeleteAsync(Guid id);
    Task<bool> InscreverGrupoAsync(Guid guiaId, Guid grupoId);
    Task<bool> RemoverGrupoAsync(Guid guiaId, Guid grupoId);
    Task<RankingGuiaDto> AtualizarRankingAsync(Guid guiaId);
    Task<IEnumerable<RankingGuiaDto>> GetRankingAsync(Guid guiaId);
}
