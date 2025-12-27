using Microsoft.EntityFrameworkCore;
using PA.Application.Interfaces.Repositories;
using PA.Domain.Entities;
using PA.Infrastructure.Data.Context;

namespace PA.Infrastructure.Repositories;

public class UserGrupoRepository : IUserGrupoRepository
{
    private readonly PastoralAppDbContext _context;

    public UserGrupoRepository(PastoralAppDbContext context)
    {
        _context = context;
    }

    public async Task<UserGrupo?> GetByUserAndGrupoAsync(Guid userId, Guid grupoId)
    {
        return await _context.Set<UserGrupo>()
            .FirstOrDefaultAsync(ug => ug.UserId == userId && ug.GrupoId == grupoId);
    }

    public async Task<IEnumerable<UserGrupo>> GetByUserIdAsync(Guid userId, bool apenasAtivos = true)
    {
        var query = _context.Set<UserGrupo>()
            .Include(ug => ug.Grupo)
                .ThenInclude(g => g.Pastoral)
            .Where(ug => ug.UserId == userId);

        if (apenasAtivos)
            query = query.Where(ug => ug.IsAtivo);

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<UserGrupo>> GetByGrupoIdAsync(Guid grupoId, bool apenasAtivos = true)
    {
        var query = _context.Set<UserGrupo>()
            .Include(ug => ug.User)
            .Where(ug => ug.GrupoId == grupoId);

        if (apenasAtivos)
            query = query.Where(ug => ug.IsAtivo);

        return await query.ToListAsync();
    }

    public async Task<UserGrupo> AddAsync(UserGrupo userGrupo)
    {
        await _context.Set<UserGrupo>().AddAsync(userGrupo);
        await _context.SaveChangesAsync();
        return userGrupo;
    }

    public async Task UpdateAsync(UserGrupo userGrupo)
    {
        var tracked = _context.Entry(userGrupo);
        
        if (tracked.State == EntityState.Detached)
        {
            _context.Set<UserGrupo>().Update(userGrupo);
        }
        else
        {
            tracked.State = EntityState.Modified;
        }
        
        await _context.SaveChangesAsync();
    }

    public async Task<int> CountActiveGruposByUserAsync(Guid userId)
    {
        return await _context.Set<UserGrupo>()
            .CountAsync(ug => ug.UserId == userId && ug.IsAtivo);
    }
}
