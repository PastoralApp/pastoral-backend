using PA.Domain.Entities;

namespace PA.Domain.Interfaces;

public interface IJogoRepository
{
    Task<Jogo?> GetByIdAsync(Guid id);
    Task<IEnumerable<Jogo>> GetAllAsync();
    Task<IEnumerable<Jogo>> GetByPastoralIdAsync(Guid pastoralId);
    Task<IEnumerable<Jogo>> GetByAnoAsync(int ano);
    Task<IEnumerable<Jogo>> GetByStatusAsync(Enums.StatusJogo status);
    Task<Jogo> AddAsync(Jogo jogo);
    Task UpdateAsync(Jogo jogo);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}
