using Microsoft.EntityFrameworkCore;
using PA.Application.Interfaces.Repositories;
using PA.Domain.Entities;
using PA.Domain.Enums;
using PA.Infrastructure.Data.Context;
using GrupoEntity = PA.Domain.Entities.Grupo;

namespace PA.Infrastructure.Repositories;

public class GrupoRepository : IGrupoRepository
{
    private readonly PastoralAppDbContext _context;

    public GrupoRepository(PastoralAppDbContext context)
    {
        _context = context;
    }

    public async Task<Grupo?> GetByIdAsync(Guid id)
    {
        return await _context.Grupos
            .Include(g => g.Pastoral)
            .Include(g => g.Igreja)
            .Include(g => g.UserGrupos.Where(ug => ug.IsAtivo))
                .ThenInclude(ug => ug.User)
            .FirstOrDefaultAsync(g => g.Id == id);
    }

    public async Task<IEnumerable<Grupo>> GetAllAsync(bool incluirInativos = false)
    {
        var query = _context.Grupos
            .Include(g => g.Pastoral)
            .Include(g => g.Igreja)
            .AsQueryable();

        if (!incluirInativos)
            query = query.Where(g => g.IsActive);

        return await query.OrderBy(g => g.Name).ToListAsync();
    }

    public async Task<IEnumerable<Grupo>> GetByPastoralIdAsync(Guid pastoralId, bool incluirInativos = false)
    {
        var query = _context.Grupos
            .Include(g => g.Pastoral)
            .Include(g => g.Igreja)
            .Where(g => g.PastoralId == pastoralId);

        if (!incluirInativos)
            query = query.Where(g => g.IsActive);

        return await query.OrderBy(g => g.Name).ToListAsync();
    }

    public async Task<IEnumerable<Grupo>> GetByTipoPastoralAsync(TipoPastoral tipo, bool incluirInativos = false)
    {
        var query = _context.Grupos
            .Include(g => g.Pastoral)
            .Include(g => g.Igreja)
            .Where(g => g.Pastoral.TipoPastoral == tipo);

        if (!incluirInativos)
            query = query.Where(g => g.IsActive);

        return await query.OrderBy(g => g.Name).ToListAsync();
    }

    public async Task<Grupo> AddAsync(Grupo grupo)
    {
        await _context.Grupos.AddAsync(grupo);
        await _context.SaveChangesAsync();
        return grupo;
    }

    public async Task<Grupo> UpdateAsync(Grupo grupo)
    {
        var tracked = _context.Entry(grupo);
        
        if (tracked.State == EntityState.Detached)
        {
            _context.Grupos.Update(grupo);
        }
        else
        {
            tracked.State = EntityState.Modified;
        }
        
        await _context.SaveChangesAsync();
        return grupo;
    }

    public async Task DeleteAsync(Guid id)
    {
        var grupo = await _context.Grupos.FindAsync(id);
        if (grupo != null)
        {
            _context.Grupos.Remove(grupo);
            await _context.SaveChangesAsync();
        }
    }
}
