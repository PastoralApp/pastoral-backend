using PA.Domain.Entities;

namespace PA.Domain.Interfaces;

public interface IOlimpiadasRepository
{
    Task<Olimpiadas?> GetByIdAsync(Guid id);
    Task<Olimpiadas?> GetByIdWithModalidadesAsync(Guid id);
    Task<Olimpiadas?> GetByIdWithRankingAsync(Guid id);
    Task<IEnumerable<Olimpiadas>> GetAllAsync();
    Task<IEnumerable<Olimpiadas>> GetByPastoralIdAsync(Guid pastoralId);
    Task<IEnumerable<Olimpiadas>> GetByAnoAsync(int ano);
    Task<Olimpiadas> AddAsync(Olimpiadas olimpiadas);
    Task UpdateAsync(Olimpiadas olimpiadas);
    Task DeleteAsync(Guid id);
}
