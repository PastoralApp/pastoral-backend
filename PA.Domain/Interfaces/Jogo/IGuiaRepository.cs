using PA.Domain.Entities;

namespace PA.Domain.Interfaces;

public interface IGuiaRepository
{
    Task<Guia?> GetByIdAsync(Guid id);
    Task<Guia?> GetByIdWithProvasAsync(Guid id);
    Task<Guia?> GetByIdWithRankingAsync(Guid id);
    Task<IEnumerable<Guia>> GetAllAsync();
    Task<IEnumerable<Guia>> GetByPastoralIdAsync(Guid pastoralId);
    Task<IEnumerable<Guia>> GetByAnoAsync(int ano);
    Task<Guia> AddAsync(Guia guia);
    Task UpdateAsync(Guia guia);
    Task DeleteAsync(Guid id);
}
