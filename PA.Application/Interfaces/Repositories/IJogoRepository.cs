using PA.Domain.Entities;

namespace PA.Application.Interfaces.Repositories;

public interface IJogoRepository
{
    Task<Jogo?> GetByIdAsync(Guid id);
    Task<Jogo?> GetByIdWithDetailsAsync(Guid id);
    Task<IEnumerable<Jogo>> GetAllAsync();
    Task<Jogo> AddAsync(Jogo jogo);
    Task<Jogo> UpdateAsync(Jogo jogo);
    Task DeleteAsync(Guid id);
}
