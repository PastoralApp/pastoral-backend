using Microsoft.EntityFrameworkCore;
using PA.Application.Interfaces.Repositories;
using PA.Domain.Entities;
using PA.Infrastructure.Data.Context;
using PastoralEntity = PA.Domain.Entities.Pastoral;

namespace PA.Infrastructure.Repositories;

public class PastoralRepository : IPastoralRepository
{
    private readonly PastoralAppDbContext _context;

    public PastoralRepository(PastoralAppDbContext context)
    {
        _context = context;
    }

    public async Task<Pastoral?> GetByIdAsync(Guid id)
    {
        return await _context.Pastorais
            .Include(p => p.Grupos.Where(g => g.IsActive))
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Pastoral>> GetAllAsync(bool incluirInativas = false)
    {
        var query = _context.Pastorais
            .Include(p => p.Grupos.Where(g => g.IsActive))
            .AsQueryable();

        if (!incluirInativas)
            query = query.Where(p => p.IsActive);

        return await query.OrderBy(p => p.Name).ToListAsync();
    }

    public async Task<Pastoral> AddAsync(Pastoral pastoral)
    {
        await _context.Pastorais.AddAsync(pastoral);
        await _context.SaveChangesAsync();
        return pastoral;
    }

    public async Task<Pastoral> UpdateAsync(Pastoral pastoral)
    {
        _context.Pastorais.Update(pastoral);
        await _context.SaveChangesAsync();
        return pastoral;
    }

    public async Task DeleteAsync(Guid id)
    {
        var pastoral = await _context.Pastorais.FindAsync(id);
        if (pastoral != null)
        {
            _context.Pastorais.Remove(pastoral);
            await _context.SaveChangesAsync();
        }
    }
}
