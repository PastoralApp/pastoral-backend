using PA.Domain.Entities;

namespace PA.Domain.Interfaces;

public interface IGrupoRepository
{
    Task<Grupo?> GetByIdAsync(Guid id);
    Task<Grupo?> GetByIdWithParticipantesAsync(Guid id);
    Task<Grupo?> GetByIdWithConquistasAsync(Guid id);
    Task<IEnumerable<Grupo>> GetAllAsync();
    Task<IEnumerable<Grupo>> GetByPastoralIdAsync(Guid pastoralId);
    Task<IEnumerable<Grupo>> GetAtivosByPastoralIdAsync(Guid pastoralId);
    Task<Grupo> AddAsync(Grupo grupo);
    Task UpdateAsync(Grupo grupo);
    Task DeleteAsync(Guid id);
}
