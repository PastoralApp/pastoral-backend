using PA.Domain.Entities;

namespace PA.Application.Interfaces.Repositories;

public interface IPastoralRepository
{
    Task<Pastoral?> GetByIdAsync(Guid id);
    Task<IEnumerable<Pastoral>> GetAllAsync(bool incluirInativas = false);
    Task<Pastoral> AddAsync(Pastoral pastoral);
    Task<Pastoral> UpdateAsync(Pastoral pastoral);
    Task DeleteAsync(Guid id);
}
