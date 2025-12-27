using PA.Domain.Entities;
using PA.Domain.Interfaces;

namespace PA.Application.Interfaces.Repositories;

public interface ITagRepository : IRepository<Tag>
{
    Task<IEnumerable<Tag>> GetByUserIdAsync(Guid userId);
}
