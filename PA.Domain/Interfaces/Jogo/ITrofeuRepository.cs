using PA.Domain.Entities;

namespace PA.Domain.Interfaces;

public interface ITrofeuRepository
{
    Task<Trofeu?> GetByIdAsync(Guid id);
    Task<IEnumerable<Trofeu>> GetAllAsync();
    Task<IEnumerable<Trofeu>> GetByJogoIdAsync(Guid jogoId);
    Task<IEnumerable<Trofeu>> GetByGrupoIdAsync(Guid grupoId);
    Task<IEnumerable<Trofeu>> GetByAnoAsync(int ano);
    Task<Trofeu> AddAsync(Trofeu trofeu);
    Task UpdateAsync(Trofeu trofeu);
    Task DeleteAsync(Guid id);
}
