using PA.Domain.Entities;
using PA.Domain.Enums;
using PA.Domain.Interfaces;

namespace PA.Application.Interfaces.Repositories;

public interface IPostRepository : IRepository<Post>
{
    Task<IEnumerable<Post>> GetByAuthorIdAsync(Guid authorId);
    Task<IEnumerable<Post>> GetPinnedPostsAsync();
    Task<IEnumerable<Post>> GetRecentPostsAsync(int count = 50);
    Task<IEnumerable<Post>> GetByPastoralAsync(Guid pastoralId);
    Task<IEnumerable<Post>> GetByTipoPastoralAsync(TipoPastoral tipoPastoral);
    Task<IEnumerable<Post>> GetByGrupoAsync(Guid grupoId);
    Task<IEnumerable<Post>> GetByTypeAsync(PostType type, int count = 10);
    Task<PostSalvo?> GetPostSalvoAsync(Guid postId, Guid userId);
    Task AddPostSalvoAsync(PostSalvo postSalvo);
    Task RemovePostSalvoAsync(PostSalvo postSalvo);
    Task<IEnumerable<Post>> GetSavedPostsByUserAsync(Guid userId);
    
    Task<PostReaction?> GetReactionAsync(Guid postId, Guid userId);
    Task AddReactionAsync(PostReaction reaction);
    Task RemoveReactionAsync(PostReaction reaction);
    
    Task<PostComment?> GetCommentByIdAsync(Guid commentId);
    Task<IEnumerable<PostComment>> GetCommentsByPostIdAsync(Guid postId);
    Task<PostComment> AddCommentAsync(PostComment comment);
    Task RemoveCommentAsync(PostComment comment);
    
    Task AddShareAsync(PostShare share);
}
