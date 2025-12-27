using PA.Domain.Entities;

namespace PA.Domain.Interfaces;

public interface IPartidaRepository
{
    Task<Partida?> GetByIdAsync(Guid id);
    Task<IEnumerable<Partida>> GetAllAsync();
    Task<IEnumerable<Partida>> GetByModalidadeIdAsync(Guid modalidadeId);
    Task<IEnumerable<Partida>> GetByGrupoIdAsync(Guid grupoId);
    Task<IEnumerable<Partida>> GetPendentesAsync();
    Task<Partida> AddAsync(Partida partida);
    Task UpdateAsync(Partida partida);
    Task DeleteAsync(Guid id);
}
