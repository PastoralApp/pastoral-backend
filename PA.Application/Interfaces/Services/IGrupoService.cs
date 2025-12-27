using PA.Application.DTOs;

namespace PA.Application.Interfaces.Services;

public interface IGrupoService
{
    Task<GrupoDto?> GetByIdAsync(Guid id);
    Task<GrupoDto?> GetByIdWithParticipantesAsync(Guid id);
    Task<GrupoDto?> GetByIdWithConquistasAsync(Guid id);
    Task<IEnumerable<GrupoDto>> GetAllAsync();
    Task<IEnumerable<GrupoDto>> GetByPastoralIdAsync(Guid pastoralId);
    Task<IEnumerable<GrupoDto>> GetAtivosByPastoralIdAsync(Guid pastoralId);
    Task<GrupoDto> CreateAsync(CreateGrupoDto dto);
    Task<GrupoDto> UpdateAsync(Guid id, UpdateGrupoDto dto);
    Task DeleteAsync(Guid id);
    Task<bool> AdicionarParticipanteAsync(AdicionarParticipanteGrupoDto dto);
    Task<bool> RemoverParticipanteAsync(RemoverParticipanteGrupoDto dto);
    Task<IEnumerable<MedalhaDto>> GetMedalhasAsync(Guid grupoId);
    Task<IEnumerable<TrofeuDto>> GetTrofeusAsync(Guid grupoId);
}
