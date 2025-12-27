using PA.Application.DTOs;
using PA.Domain.Enums;

namespace PA.Application.Interfaces.Services;

public interface IPostService
{
    Task<PostDto?> GetByIdAsync(Guid id);
    Task<PostDto?> GetByIdAsync(Guid id, Guid currentUserId);
    Task<PostDetailDto?> GetPostDetailAsync(Guid id, Guid currentUserId);
    Task<IEnumerable<PostDto>> GetRecentAsync(int count = 50);
    Task<IEnumerable<PostDto>> GetRecentAsync(int count, Guid currentUserId);
    Task<IEnumerable<PostDto>> GetPinnedAsync();
    Task<IEnumerable<PostDto>> GetByPastoralAsync(Guid pastoralId);
    Task<IEnumerable<PostDto>> GetByTipoPastoralAsync(TipoPastoral tipoPastoral);
    Task<IEnumerable<PostDto>> GetByGrupoAsync(Guid grupoId);
    Task<IEnumerable<PostDto>> GetByUserAsync(Guid userId);
    Task<PostDto> CreateAsync(CreatePostDto dto, Guid authorId);
    Task UpdateAsync(Guid id, UpdatePostDto dto);
    Task DeleteAsync(Guid id);
    Task PinAsync(Guid id, Guid userId, string pinType = "Geral");
    Task UnpinAsync(Guid id, Guid userId);
    Task<(bool reacted, int likesCount)> ToggleReactionAsync(Guid postId, Guid userId);
    Task<CommentDto> AddCommentAsync(Guid postId, Guid userId, string conteudo);
    Task DeleteCommentAsync(Guid commentId, Guid userId);
    Task<IEnumerable<CommentDto>> GetCommentsAsync(Guid postId);
    Task<int> ShareAsync(Guid postId, Guid userId);
    Task<bool> ToggleSavePostAsync(Guid postId, Guid userId);
    Task<IEnumerable<PostDto>> GetSavedPostsAsync(Guid userId);
    Task ChangeTypeAsync(Guid postId, PostType newType);
    Task<IEnumerable<PostDto>> GetByTypeAsync(PostType type, int count = 10);
}
