using PA.Domain.Entities;

namespace PA.Domain.Interfaces;

public interface IPontuacaoProvaRepository
{
    Task<PontuacaoProva?> GetByIdAsync(Guid id);
    Task<IEnumerable<PontuacaoProva>> GetByProvaIdAsync(Guid provaId);
    Task<IEnumerable<PontuacaoProva>> GetByGrupoIdAsync(Guid grupoId);
    Task<PontuacaoProva> AddAsync(PontuacaoProva pontuacao);
    Task UpdateAsync(PontuacaoProva pontuacao);
    Task DeleteAsync(Guid id);
}
