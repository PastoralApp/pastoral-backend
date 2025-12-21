using PA.Domain.Entities;

namespace PA.Application.Interfaces.Repositories;

public interface IIgrejaRepository
{
    Task<Igreja?> GetByIdAsync(Guid id);
    Task<IEnumerable<Igreja>> GetAllAsync(bool incluirInativas = false);
    Task<Igreja> AddAsync(Igreja igreja);
    Task<Igreja> UpdateAsync(Igreja igreja);
    Task DeleteAsync(Guid id);
}
