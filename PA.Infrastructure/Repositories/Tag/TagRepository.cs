using Microsoft.EntityFrameworkCore;
using PA.Application.Interfaces.Repositories;
using PA.Domain.Entities;
using PA.Infrastructure.Data.Context;

namespace PA.Infrastructure.Repositories;

public class TagRepository : ITagRepository
{
    private readonly PastoralAppDbContext _context;

    public TagRepository(PastoralAppDbContext context)
    {
        _context = context;
    }

    public async Task<Tag?> GetByIdAsync(Guid id)
    {
        return await _context.Tags
            .Include(t => t.Users)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<Tag>> GetAllAsync()
    {
        return await _context.Tags
            .Include(t => t.Users)
            .ToListAsync();
    }

    public async Task<IEnumerable<Tag>> FindAsync(System.Linq.Expressions.Expression<Func<Tag, bool>> predicate)
    {
        return await _context.Tags
            .Where(predicate)
            .ToListAsync();
    }

    public async Task<Tag> AddAsync(Tag entity)
    {
        await _context.Tags.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(Tag entity)
    {
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var tag = await _context.Tags.FindAsync(id);
        if (tag != null)
        {
            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Tags.AnyAsync(t => t.Id == id);
    }

    public async Task<int> CountAsync()
    {
        return await _context.Tags.CountAsync();
    }

    public async Task<IEnumerable<Tag>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Tags
            .Where(t => t.Users.Any(u => u.Id == userId))
            .ToListAsync();
    }
}
