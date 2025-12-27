using PA.Domain.Entities;

namespace PA.Domain.Interfaces;

public interface IProvaRepository
{
    Task<Prova?> GetByIdAsync(Guid id);
    Task<Prova?> GetByIdWithPontuacoesAsync(Guid id);
    Task<IEnumerable<Prova>> GetAllAsync();
    Task<IEnumerable<Prova>> GetByGuiaIdAsync(Guid guiaId);
    Task<IEnumerable<Prova>> GetByGuiaIdOrderedAsync(Guid guiaId);
    Task<Prova> AddAsync(Prova prova);
    Task UpdateAsync(Prova prova);
    Task DeleteAsync(Guid id);
}
