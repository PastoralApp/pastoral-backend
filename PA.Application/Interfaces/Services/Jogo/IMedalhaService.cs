using PA.Application.DTOs;
using PA.Domain.Enums;

namespace PA.Application.Interfaces.Services;

public interface IMedalhaService
{
    Task<MedalhaDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<MedalhaDto>> GetAllAsync();
    Task<IEnumerable<MedalhaDto>> GetByJogoIdAsync(Guid jogoId);
    Task<IEnumerable<MedalhaDto>> GetByGrupoIdAsync(Guid grupoId);
    Task<IEnumerable<MedalhaDto>> GetByParticipanteIdAsync(Guid participanteId);
    Task<IEnumerable<MedalhaDto>> GetByAnoAsync(int ano);
    Task<IEnumerable<MedalhaDto>> GetByTipoAsync(TipoMedalha tipo);
    Task<MedalhaDto> CreateAsync(CreateMedalhaDto dto);
    Task DeleteAsync(Guid id);
}
