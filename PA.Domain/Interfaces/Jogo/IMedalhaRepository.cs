using PA.Domain.Entities;

namespace PA.Domain.Interfaces;

public interface IMedalhaRepository
{
    Task<Medalha?> GetByIdAsync(Guid id);
    Task<IEnumerable<Medalha>> GetAllAsync();
    Task<IEnumerable<Medalha>> GetByJogoIdAsync(Guid jogoId);
    Task<IEnumerable<Medalha>> GetByGrupoIdAsync(Guid grupoId);
    Task<IEnumerable<Medalha>> GetByParticipanteIdAsync(Guid participanteId);
    Task<IEnumerable<Medalha>> GetByAnoAsync(int ano);
    Task<Medalha> AddAsync(Medalha medalha);
    Task UpdateAsync(Medalha medalha);
    Task DeleteAsync(Guid id);
}
