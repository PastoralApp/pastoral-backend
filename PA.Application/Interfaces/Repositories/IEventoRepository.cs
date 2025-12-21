using PA.Domain.Entities;
using PA.Domain.Interfaces;

namespace PA.Application.Interfaces.Repositories;

public interface IEventoRepository : IRepository<Evento>
{
    Task<IEnumerable<Evento>> GetUpcomingEventosAsync();
    Task<IEnumerable<Evento>> GetPastEventosAsync();
    Task<IEnumerable<Evento>> GetByCreatorIdAsync(Guid creatorId);
    Task<EventoSalvo?> GetEventoSalvoAsync(Guid eventoId, Guid userId);
    Task AddEventoSalvoAsync(EventoSalvo eventoSalvo);
    Task RemoveEventoSalvoAsync(EventoSalvo eventoSalvo);
    Task<IEnumerable<Evento>> GetSavedEventosByUserAsync(Guid userId);
}
