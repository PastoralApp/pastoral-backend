using PA.Domain.Entities;

namespace PA.Domain.Interfaces;

public interface IRankingGuiaRepository
{
    Task<RankingGuia?> GetByIdAsync(Guid id);
    Task<IEnumerable<RankingGuia>> GetByGuiaIdAsync(Guid guiaId);
    Task<RankingGuia> AddAsync(RankingGuia ranking);
    Task UpdateAsync(RankingGuia ranking);
    Task DeleteAsync(Guid id);
    Task RecalcularRankingAsync(Guid guiaId);
}
