using PA.Domain.Entities;

namespace PA.Application.Interfaces.Repositories;

public interface IOlimpiadasRepository
{
    Task<Olimpiadas?> GetByIdAsync(Guid id);
    Task<Olimpiadas?> GetByIdWithDetailsAsync(Guid id);
    Task<IEnumerable<Olimpiadas>> GetAllAsync();
    Task<Olimpiadas> AddAsync(Olimpiadas olimpiadas);
    Task<Olimpiadas> UpdateAsync(Olimpiadas olimpiadas);
    Task DeleteAsync(Guid id);
}
