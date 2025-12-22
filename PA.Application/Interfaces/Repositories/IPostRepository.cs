using PA.Domain.Entities;
using PA.Domain.Interfaces;

namespace PA.Application.Interfaces.Repositories;

public interface IPostRepository : IRepository<Post>
{
    Task<IEnumerable<Post>> GetByAuthorIdAsync(Guid authorId);
    Task<IEnumerable<Post>> GetPinnedPostsAsync();
    Task<IEnumerable<Post>> GetRecentPostsAsync(int count = 50);
    Task<IEnumerable<Post>> GetByPastoralAsync(Guid pastoralId);
    Task<IEnumerable<Post>> GetByGrupoAsync(Guid grupoId);
    Task<PostSalvo?> GetPostSalvoAsync(Guid postId, Guid userId);
    Task AddPostSalvoAsync(PostSalvo postSalvo);
    Task RemovePostSalvoAsync(PostSalvo postSalvo);
    Task<IEnumerable<Post>> GetSavedPostsByUserAsync(Guid userId);
}
