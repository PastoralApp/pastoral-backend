using PA.Domain.Entities;
using PA.Domain.Enums;

namespace PA.Application.Interfaces.Repositories;

public interface IGrupoRepository
{
    Task<Grupo?> GetByIdAsync(Guid id);
    Task<IEnumerable<Grupo>> GetAllAsync(bool incluirInativos = false);
    Task<IEnumerable<Grupo>> GetByPastoralIdAsync(Guid pastoralId, bool incluirInativos = false);
    Task<IEnumerable<Grupo>> GetByTipoPastoralAsync(TipoPastoral tipo, bool incluirInativos = false);
    Task<Grupo> AddAsync(Grupo grupo);
    Task<Grupo> UpdateAsync(Grupo grupo);
    Task DeleteAsync(Guid id);
}
