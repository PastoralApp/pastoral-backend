using PA.Domain.Entities;
using PA.Domain.Interfaces;

namespace PA.Application.Interfaces.Repositories;

/// <summary>
/// Interface de repositório para Post
/// </summary>
public interface IPostRepository : IRepository<Post>
{
    Task<IEnumerable<Post>> GetByAuthorIdAsync(Guid authorId);
    Task<IEnumerable<Post>> GetPinnedPostsAsync();
    Task<IEnumerable<Post>> GetRecentPostsAsync(int count = 50);
}
