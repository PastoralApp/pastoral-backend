using Microsoft.EntityFrameworkCore;
using PA.Application.Interfaces.Repositories;
using PA.Domain.Entities;
using PA.Domain.Enums;
using PA.Infrastructure.Data.Context;

namespace PA.Infrastructure.Repositories;

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
            .Include(e => e.Participantes)
            .Include(e => e.Grupo)
            .Include(e => e.ResponsavelUser)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<IEnumerable<Evento>> GetAllAsync()
    {
        return await _context.Eventos
            .Include(e => e.CreatedBy)
            .Include(e => e.Participantes)
            .Include(e => e.Grupo)
            .Include(e => e.ResponsavelUser)
            .OrderBy(e => e.EventDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Evento>> FindAsync(System.Linq.Expressions.Expression<Func<Evento, bool>> predicate)
    {
        return await _context.Eventos
            .Include(e => e.CreatedBy)
            .Include(e => e.Participantes)
            .Include(e => e.Grupo)
            .Include(e => e.ResponsavelUser)
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
            .Include(e => e.Participantes)
            .Include(e => e.Grupo)
            .Include(e => e.ResponsavelUser)
            .Where(e => e.EventDate >= now)
            .OrderBy(e => e.EventDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Evento>> GetPastEventosAsync()
    {
        var now = DateTime.UtcNow;
        return await _context.Eventos
            .Include(e => e.CreatedBy)
            .Include(e => e.Participantes)
            .Include(e => e.Grupo)
            .Include(e => e.ResponsavelUser)
            .Where(e => e.EventDate < now)
            .OrderByDescending(e => e.EventDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Evento>> GetByCreatorIdAsync(Guid creatorId)
    {
        return await _context.Eventos
            .Include(e => e.CreatedBy)
            .Include(e => e.Participantes)
            .Include(e => e.Grupo)
            .Include(e => e.ResponsavelUser)
            .Where(e => e.CreatedByUserId == creatorId)
            .OrderBy(e => e.EventDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Evento>> GetByGrupoAsync(Guid grupoId)
    {
        return await _context.Eventos
            .Include(e => e.CreatedBy)
            .Include(e => e.Participantes)
            .Include(e => e.Grupo)
            .Include(e => e.ResponsavelUser)
            .Where(e => e.GrupoId == grupoId || e.CreatedBy.UserGrupos.Any(ug => ug.GrupoId == grupoId && ug.IsAtivo))
            .OrderBy(e => e.EventDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Evento>> GetByPastoralAsync(Guid pastoralId)
    {
        return await _context.Eventos
            .Include(e => e.CreatedBy)
            .Include(e => e.Participantes)
            .Where(e => e.CreatedBy.UserGrupos.Any(ug => ug.Grupo.PastoralId == pastoralId && ug.IsAtivo))
            .OrderBy(e => e.EventDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Evento>> GetByTypeAsync(EventoType type)
    {
        return await _context.Eventos
            .Include(e => e.CreatedBy)
            .Include(e => e.Participantes)
            .Where(e => e.Type == type)
            .OrderBy(e => e.EventDate)
            .ToListAsync();
    }

    public async Task<Evento?> GetByIdWithParticipantesAsync(Guid eventoId)
    {
        return await _context.Eventos
            .Include(e => e.CreatedBy)
            .Include(e => e.Participantes)
                .ThenInclude(p => p.User)
            .FirstOrDefaultAsync(e => e.Id == eventoId);
    }

    public async Task<EventoSalvo?> GetEventoSalvoAsync(Guid eventoId, Guid userId)
    {
        return await _context.Set<EventoSalvo>()
            .FirstOrDefaultAsync(es => es.EventoId == eventoId && es.UserId == userId);
    }

    public async Task AddEventoSalvoAsync(EventoSalvo eventoSalvo)
    {
        await _context.Set<EventoSalvo>().AddAsync(eventoSalvo);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveEventoSalvoAsync(EventoSalvo eventoSalvo)
    {
        _context.Set<EventoSalvo>().Remove(eventoSalvo);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Evento>> GetSavedEventosByUserAsync(Guid userId)
    {
        return await _context.Set<EventoSalvo>()
            .Include(es => es.Evento).ThenInclude(e => e.CreatedBy)
            .Where(es => es.UserId == userId)
            .OrderByDescending(es => es.DataSalvamento)
            .Select(es => es.Evento)
            .ToListAsync();
    }
}
