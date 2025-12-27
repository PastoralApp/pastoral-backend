using PA.Domain.Entities;

namespace PA.Domain.Interfaces;

public interface IPontuacaoJogoRepository
{
    Task<PontuacaoJogo?> GetByIdAsync(Guid id);
    Task<IEnumerable<PontuacaoJogo>> GetByJogoIdAsync(Guid jogoId);
    Task<IEnumerable<PontuacaoJogo>> GetByGrupoIdAsync(Guid grupoId);
    Task<PontuacaoJogo?> GetByJogoAndGrupoAsync(Guid jogoId, Guid grupoId);
    Task<PontuacaoJogo> AddAsync(PontuacaoJogo pontuacao);
    Task UpdateAsync(PontuacaoJogo pontuacao);
    Task DeleteAsync(Guid id);
}
