using Microsoft.EntityFrameworkCore;
using PA.Application.Interfaces.Repositories;
using PA.Domain.Entities;
using PA.Infrastructure.Data.Context;

namespace PA.Infrastructure.Repositories;

public class OlimpiadasRepository : IOlimpiadasRepository
{
    private readonly PastoralAppDbContext _context;
    public OlimpiadasRepository(PastoralAppDbContext context)
    {
        _context = context;
    }

    public async Task<Olimpiadas?> GetByIdAsync(Guid id)
    {
        return await _context.Olimpiadas.FindAsync(id);
    }

    public async Task<Olimpiadas?> GetByIdWithDetailsAsync(Guid id)
    {
        return await _context.Olimpiadas
            .Include(o => o.Modalidades)
            .Include(o => o.GruposParticipantes)
                .ThenInclude(gj => gj.Grupo)
            .Include(o => o.Rankings)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<IEnumerable<Olimpiadas>> GetAllAsync()
    {
        return await _context.Olimpiadas.ToListAsync();
    }

    public async Task<Olimpiadas> AddAsync(Olimpiadas olimpiadas)
    {
        _context.Olimpiadas.Add(olimpiadas);
        await _context.SaveChangesAsync();
        return olimpiadas;
    }

    public async Task<Olimpiadas> UpdateAsync(Olimpiadas olimpiadas)
    {
        _context.Olimpiadas.Update(olimpiadas);
        await _context.SaveChangesAsync();
        return olimpiadas;
    }

    public async Task DeleteAsync(Guid id)
    {
        var olimpiadas = await _context.Olimpiadas.FindAsync(id);
        if (olimpiadas != null)
        {
            _context.Olimpiadas.Remove(olimpiadas);
            await _context.SaveChangesAsync();
        }
    }
}
