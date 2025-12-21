using Microsoft.EntityFrameworkCore;
using PA.Application.Interfaces.Repositories;
using PA.Domain.Entities;
using PA.Infrastructure.Data.Context;

namespace PA.Infrastructure.Repositories;

/// <summary>
/// Implementação do repositório de Evento
/// </summary>
public class EventoRepository : IEventoRepository
{
    private readonly PastoralAppDbContext _context;

    public EventoRepository(PastoralAppDbContext context)
    {
        _context = context;
    }

    public async Task<Evento?> GetByIdAsync(Guid id)
    {
        return await _context.Eventos
            .Include(e => e.CreatedBy)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<IEnumerable<Evento>> GetAllAsync()
    {
        return await _context.Eventos
            .Include(e => e.CreatedBy)
            .OrderBy(e => e.EventDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Evento>> FindAsync(System.Linq.Expressions.Expression<Func<Evento, bool>> predicate)
    {
        return await _context.Eventos
            .Include(e => e.CreatedBy)
            .Where(predicate)
            .ToListAsync();
    }

    public async Task<Evento> AddAsync(Evento entity)
    {
        await _context.Eventos.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(Evento entity)
    {
        _context.Eventos.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var evento = await GetByIdAsync(id);
        if (evento != null)
        {
            _context.Eventos.Remove(evento);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Eventos.AnyAsync(e => e.Id == id);
    }

    public async Task<int> CountAsync()
    {
        return await _context.Eventos.CountAsync();
    }

    public async Task<IEnumerable<Evento>> GetUpcomingEventosAsync()
    {
        var now = DateTime.UtcNow;
        return await _context.Eventos
            .Include(e => e.CreatedBy)
            .Where(e => e.EventDate >= now)
            .OrderBy(e => e.EventDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Evento>> GetPastEventosAsync()
    {
        var now = DateTime.UtcNow;
        return await _context.Eventos
            .Include(e => e.CreatedBy)
            .Where(e => e.EventDate < now)
            .OrderByDescending(e => e.EventDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Evento>> GetByCreatorIdAsync(Guid creatorId)
    {
        return await _context.Eventos
            .Include(e => e.CreatedBy)
            .Where(e => e.CreatedByUserId == creatorId)
            .OrderBy(e => e.EventDate)
            .ToListAsync();
    }
}
