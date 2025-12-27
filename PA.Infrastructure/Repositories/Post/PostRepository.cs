using Microsoft.EntityFrameworkCore;
using PA.Application.Interfaces.Repositories;
using PA.Domain.Entities;
using PA.Domain.Enums;
using PA.Infrastructure.Data.Context;

namespace PA.Infrastructure.Repositories;

public class PostRepository : IPostRepository
{
    private readonly PastoralAppDbContext _context;

    public PastoralAppDbContext Context => _context;

    public PostRepository(PastoralAppDbContext context)
    {
        _context = context;
    }

    public async Task<Post?> GetByIdAsync(Guid id)
    {
        return await _context.Posts
            .Include(p => p.Author)
            .Include(p => p.Reactions)
            .Include(p => p.Comments).ThenInclude(c => c.User)
            .Include(p => p.Shares)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Post>> GetAllAsync()
    {
        return await _context.Posts
            .Include(p => p.Author)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Post>> FindAsync(System.Linq.Expressions.Expression<Func<Post, bool>> predicate)
    {
        return await _context.Posts
            .Include(p => p.Author)
            .Where(predicate)
            .ToListAsync();
    }

    public async Task<Post> AddAsync(Post entity)
    {
        await _context.Posts.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(Post entity)
    {
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var post = await _context.Posts.FindAsync(id);
        if (post != null)
        {
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Posts.AnyAsync(p => p.Id == id);
    }

    public async Task<int> CountAsync()
    {
        return await _context.Posts.CountAsync();
    }

    public async Task<IEnumerable<Post>> GetByAuthorIdAsync(Guid authorId)
    {
        return await _context.Posts
            .Include(p => p.Author)
            .Where(p => p.AuthorId == authorId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Post>> GetPinnedPostsAsync()
    {
        var posts = await _context.Posts
            .Include(p => p.Author)
            .Where(p => p.IsPinned)
            .ToListAsync();
        
        return posts.OrderByDescending(p => GetPinPriority(p.PinType))
                   .ThenByDescending(p => p.CreatedAt);
    }

    private int GetPinPriority(string? pinType)
    {
        return pinType switch
        {
            "Admin" => 3,
            "Coordenador Geral" => 2,
            "Coordenador de Grupo" => 1,
            _ => 0
        };
    }

    public async Task<IEnumerable<Post>> GetRecentPostsAsync(int count = 50)
    {
        return await _context.Posts
            .Include(p => p.Author)
            .OrderByDescending(p => p.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<Post>> GetByPastoralAsync(Guid pastoralId)
    {
        var posts = await _context.Posts
            .Include(p => p.Author)
            .Include(p => p.Reactions)
            .Include(p => p.Comments)
            .Where(p => p.PastoralId == pastoralId)
            .ToListAsync();
        
        return posts.OrderByDescending(p => p.IsPinned)
                   .ThenByDescending(p => GetPinPriority(p.PinType))
                   .ThenByDescending(p => p.CreatedAt);
    }

    public async Task<IEnumerable<Post>> GetByTipoPastoralAsync(TipoPastoral tipoPastoral)
    {
        var posts = await _context.Posts
            .Include(p => p.Author)
            .Include(p => p.Reactions)
            .Include(p => p.Comments)
            .Where(p => p.TipoPastoral == tipoPastoral)
            .ToListAsync();
        
        return posts.OrderByDescending(p => p.IsPinned)
                   .ThenByDescending(p => GetPinPriority(p.PinType))
                   .ThenByDescending(p => p.CreatedAt);
    }

    public async Task<IEnumerable<Post>> GetByGrupoAsync(Guid grupoId)
    {
        return await _context.Posts
            .Include(p => p.Author)
            .Include(p => p.Reactions)
            .Include(p => p.Comments)
            .Where(p => p.Author.UserGrupos.Any(ug => ug.GrupoId == grupoId && ug.IsAtivo))
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<PostSalvo?> GetPostSalvoAsync(Guid postId, Guid userId)
    {
        return await _context.Set<PostSalvo>()
            .FirstOrDefaultAsync(ps => ps.PostId == postId && ps.UserId == userId);
    }

    public async Task AddPostSalvoAsync(PostSalvo postSalvo)
    {
        await _context.Set<PostSalvo>().AddAsync(postSalvo);
        await _context.SaveChangesAsync();
    }

    public async Task RemovePostSalvoAsync(PostSalvo postSalvo)
    {
        _context.Set<PostSalvo>().Remove(postSalvo);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Post>> GetSavedPostsByUserAsync(Guid userId)
    {
        return await _context.Set<PostSalvo>()
            .Include(ps => ps.Post).ThenInclude(p => p.Author)
            .Include(ps => ps.Post).ThenInclude(p => p.Reactions)
            .Include(ps => ps.Post).ThenInclude(p => p.Comments)
            .Where(ps => ps.UserId == userId)
            .OrderByDescending(ps => ps.DataSalvamento)
            .Select(ps => ps.Post)
            .ToListAsync();
    }

    public async Task<PostReaction?> GetReactionAsync(Guid postId, Guid userId)
    {
        return await _context.Set<PostReaction>()
            .FirstOrDefaultAsync(r => r.PostId == postId && r.UserId == userId);
    }

    public async Task AddReactionAsync(PostReaction reaction)
    {
        await _context.Set<PostReaction>().AddAsync(reaction);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveReactionAsync(PostReaction reaction)
    {
        _context.Set<PostReaction>().Remove(reaction);
        await _context.SaveChangesAsync();
    }

    public async Task<PostComment?> GetCommentByIdAsync(Guid commentId)
    {
        return await _context.Set<PostComment>()
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.Id == commentId);
    }

    public async Task<IEnumerable<PostComment>> GetCommentsByPostIdAsync(Guid postId)
    {
        return await _context.Set<PostComment>()
            .Include(c => c.User)
            .Where(c => c.PostId == postId && c.IsAtivo)
            .OrderBy(c => c.DataComentario)
            .ToListAsync();
    }

    public async Task<PostComment> AddCommentAsync(PostComment comment)
    {
        await _context.Set<PostComment>().AddAsync(comment);
        await _context.SaveChangesAsync();
        
        return (await GetCommentByIdAsync(comment.Id))!;
    }

    public async Task RemoveCommentAsync(PostComment comment)
    {
        _context.Set<PostComment>().Remove(comment);
        await _context.SaveChangesAsync();
    }

    public async Task AddShareAsync(PostShare share)
    {
        await _context.Set<PostShare>().AddAsync(share);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Post>> GetByTypeAsync(PostType type, int count = 10)
    {
        return await _context.Posts
            .Include(p => p.Author)
            .Where(p => p.Type == type)
            .OrderByDescending(p => p.CreatedAt)
            .Take(count)
            .ToListAsync();
    }
}

