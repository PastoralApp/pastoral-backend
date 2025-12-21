using PA.Domain.Entities;
using PA.Domain.Interfaces;

namespace PA.Application.Interfaces.Repositories;

/// <summary>
/// Interface de repositório para Evento
/// </summary>
public interface IEventoRepository : IRepository<Evento>
{
    Task<IEnumerable<Evento>> GetUpcomingEventosAsync();
    Task<IEnumerable<Evento>> GetPastEventosAsync();
    Task<IEnumerable<Evento>> GetByCreatorIdAsync(Guid creatorId);
}
