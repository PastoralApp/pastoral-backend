using PA.Domain.Entities;

namespace PA.Domain.Interfaces;

public interface IRankingOlimpiadasRepository
{
    Task<RankingOlimpiadas?> GetByIdAsync(Guid id);
    Task<IEnumerable<RankingOlimpiadas>> GetByOlimpiadasIdAsync(Guid olimpiadasId);
    Task<RankingOlimpiadas> AddAsync(RankingOlimpiadas ranking);
    Task UpdateAsync(RankingOlimpiadas ranking);
    Task DeleteAsync(Guid id);
    Task RecalcularRankingAsync(Guid olimpiadasId);
}
