using PA.Domain.Entities;

namespace PA.Application.Interfaces.Repositories;

public interface IGuiaRepository
{
    Task<Guia?> GetByIdAsync(Guid id);
    Task<Guia?> GetByIdWithDetailsAsync(Guid id);
    Task<IEnumerable<Guia>> GetAllAsync();
    Task<Guia> AddAsync(Guia guia);
    Task<Guia> UpdateAsync(Guia guia);
    Task DeleteAsync(Guid id);
}
