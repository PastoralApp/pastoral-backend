using PA.Application.DTOs;

namespace PA.Application.Interfaces.Services;

public interface IEventoService
{
    Task<EventoDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<EventoDto>> GetUpcomingAsync();
    Task<IEnumerable<EventoDto>> GetPastAsync();
    Task<IEnumerable<EventoDto>> GetByGrupoAsync(Guid grupoId);
    Task<IEnumerable<EventoDto>> GetByPastoralAsync(Guid pastoralId);
    Task<EventoDto> CreateAsync(CreateEventoDto dto, Guid userId);
    Task UpdateAsync(Guid id, UpdateEventoDto dto);
    Task DeleteAsync(Guid id);
}
