using Microsoft.EntityFrameworkCore;
using PA.Application.Interfaces.Repositories;
using PA.Domain.Entities;
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
        _context.Posts.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var post = await GetByIdAsync(id);
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
        return await _context.Posts
            .Include(p => p.Author)
            .Where(p => p.IsPinned)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
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
        return await _context.Posts
            .Include(p => p.Author)
            .Include(p => p.Reactions)
            .Include(p => p.Comments)
            .Where(p => p.Author.UserGrupos.Any(ug => ug.Grupo.PastoralId == pastoralId && ug.IsAtivo))
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
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
}
