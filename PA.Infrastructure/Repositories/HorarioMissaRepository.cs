using Microsoft.EntityFrameworkCore;
using PA.Application.Interfaces.Repositories;
using PA.Domain.Entities;
using PA.Infrastructure.Data.Context;

namespace PA.Infrastructure.Repositories;

public class HorarioMissaRepository : IHorarioMissaRepository
{
    private readonly PastoralAppDbContext _context;

    public HorarioMissaRepository(PastoralAppDbContext context)
    {
        _context = context;
    }

    public async Task<HorarioMissa?> GetByIdAsync(Guid id)
    {
        return await _context.HorariosMissas
            .Include(h => h.Igreja)
            .FirstOrDefaultAsync(h => h.Id == id);
    }

    public async Task<IEnumerable<HorarioMissa>> GetAllAsync(bool incluirInativos = false)
    {
        var query = _context.HorariosMissas
            .Include(h => h.Igreja)
            .AsQueryable();

        if (!incluirInativos)
            query = query.Where(h => h.IsAtivo);

        return await query
            .OrderBy(h => h.DiaSemana)
            .ThenBy(h => h.Horario)
            .ToListAsync();
    }

    public async Task<IEnumerable<HorarioMissa>> GetByIgrejaIdAsync(Guid igrejaId, bool incluirInativos = false)
    {
        var query = _context.HorariosMissas
            .Include(h => h.Igreja)
            .Where(h => h.IgrejaId == igrejaId);

        if (!incluirInativos)
            query = query.Where(h => h.IsAtivo);

        return await query
            .OrderBy(h => h.DiaSemana)
            .ThenBy(h => h.Horario)
            .ToListAsync();
    }

    public async Task<IEnumerable<HorarioMissa>> GetByDiaSemanaAsync(DayOfWeek diaSemana, bool incluirInativos = false)
    {
        var query = _context.HorariosMissas
            .Include(h => h.Igreja)
            .Where(h => h.DiaSemana == diaSemana);

        if (!incluirInativos)
            query = query.Where(h => h.IsAtivo);

        return await query
            .OrderBy(h => h.Horario)
            .ToListAsync();
    }

    public async Task<HorarioMissa> AddAsync(HorarioMissa horarioMissa)
    {
        await _context.HorariosMissas.AddAsync(horarioMissa);
        await _context.SaveChangesAsync();
        return horarioMissa;
    }

    public async Task<HorarioMissa> UpdateAsync(HorarioMissa horarioMissa)
    {
        _context.HorariosMissas.Update(horarioMissa);
        await _context.SaveChangesAsync();
        return horarioMissa;
    }

    public async Task DeleteAsync(Guid id)
    {
        var horario = await _context.HorariosMissas.FindAsync(id);
        if (horario != null)
        {
            _context.HorariosMissas.Remove(horario);
            await _context.SaveChangesAsync();
        }
    }
}
