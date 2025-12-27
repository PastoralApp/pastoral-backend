using PA.Application.DTOs;
using PA.Domain.Enums;

namespace PA.Application.Interfaces.Services;

public interface IEventoService
{
    Task<EventoDto?> GetByIdAsync(Guid id, Guid? userId = null);
    Task<IEnumerable<EventoDto>> GetUpcomingAsync(Guid? userId = null);
    Task<IEnumerable<EventoDto>> GetPastAsync();
    Task<IEnumerable<EventoDto>> GetByGrupoAsync(Guid grupoId);
    Task<IEnumerable<EventoDto>> GetByPastoralAsync(Guid pastoralId);
    Task<IEnumerable<EventoDto>> GetByTypeAsync(EventoType type);
    Task<EventoDto> CreateAsync(CreateEventoDto dto, Guid userId);
    Task UpdateAsync(Guid id, UpdateEventoDto dto);
    Task DeleteAsync(Guid id);
    Task<bool> ParticiparAsync(Guid eventoId, Guid userId);
    Task<bool> CancelarParticipacaoAsync(Guid eventoId, Guid userId);
    Task<IEnumerable<EventoParticipanteDto>> GetParticipantesAsync(Guid eventoId);
}
