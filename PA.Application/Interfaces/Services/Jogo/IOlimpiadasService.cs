using PA.Application.DTOs;
using PA.Domain.Enums;

namespace PA.Application.Interfaces.Services;

public interface IOlimpiadasService
{
    Task<OlimpiadasDto?> GetByIdAsync(Guid id);
    Task<OlimpiadasDto?> GetByIdWithDetailsAsync(Guid id);
    Task<IEnumerable<OlimpiadasDto>> GetAllAsync();
    Task<IEnumerable<OlimpiadasDto>> GetByPastoralIdAsync(Guid pastoralId);
    Task<IEnumerable<OlimpiadasDto>> GetByAnoAsync(int ano);
    Task<IEnumerable<OlimpiadasDto>> GetByStatusAsync(StatusJogo status);
    Task<OlimpiadasDto> CreateAsync(CreateOlimpiadasDto dto, Guid userId);
    Task<OlimpiadasDto> UpdateAsync(Guid id, UpdateOlimpiadasDto dto, Guid userId);
    Task DeleteAsync(Guid id);
    Task<bool> InscreverGrupoAsync(Guid olimpiadasId, Guid grupoId);
    Task<bool> RemoverGrupoAsync(Guid olimpiadasId, Guid grupoId);
    Task<RankingOlimpiadasDto> AtualizarRankingAsync(Guid olimpiadasId);
    Task<IEnumerable<RankingOlimpiadasDto>> GetRankingAsync(Guid olimpiadasId);
}
