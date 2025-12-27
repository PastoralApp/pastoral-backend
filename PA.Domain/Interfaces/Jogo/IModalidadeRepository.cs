using PA.Domain.Entities;

namespace PA.Domain.Interfaces;

public interface IModalidadeRepository
{
    Task<Modalidade?> GetByIdAsync(Guid id);
    Task<Modalidade?> GetByIdWithPartidasAsync(Guid id);
    Task<IEnumerable<Modalidade>> GetAllAsync();
    Task<IEnumerable<Modalidade>> GetByOlimpiadasIdAsync(Guid olimpiadasId);
    Task<Modalidade> AddAsync(Modalidade modalidade);
    Task UpdateAsync(Modalidade modalidade);
    Task DeleteAsync(Guid id);
}
