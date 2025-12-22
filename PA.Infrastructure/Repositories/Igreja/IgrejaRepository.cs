using Microsoft.EntityFrameworkCore;
using PA.Application.Interfaces.Repositories;
using PA.Domain.Entities;
using PA.Infrastructure.Data.Context;

namespace PA.Infrastructure.Repositories;

public class IgrejaRepository : IIgrejaRepository
{
    private readonly PastoralAppDbContext _context;

    public IgrejaRepository(PastoralAppDbContext context)
    {
        _context = context;
    }

    public async Task<Igreja?> GetByIdAsync(Guid id)
    {
        return await _context.Igrejas
            .Include(i => i.HorariosMissas.Where(h => h.IsAtivo))
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<IEnumerable<Igreja>> GetAllAsync(bool incluirInativas = false)
    {
        var query = _context.Igrejas
            .Include(i => i.HorariosMissas.Where(h => h.IsAtivo))
            .AsQueryable();

        if (!incluirInativas)
            query = query.Where(i => i.IsAtiva);

        return await query.OrderBy(i => i.Nome).ToListAsync();
    }

    public async Task<Igreja> AddAsync(Igreja igreja)
    {
        await _context.Igrejas.AddAsync(igreja);
        await _context.SaveChangesAsync();
        return igreja;
    }

    public async Task<Igreja> UpdateAsync(Igreja igreja)
    {
        _context.Igrejas.Update(igreja);
        await _context.SaveChangesAsync();
        return igreja;
    }

    public async Task DeleteAsync(Guid id)
    {
        var igreja = await _context.Igrejas.FindAsync(id);
        if (igreja != null)
        {
            _context.Igrejas.Remove(igreja);
            await _context.SaveChangesAsync();
        }
    }
}
