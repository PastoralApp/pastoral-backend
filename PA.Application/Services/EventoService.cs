using AutoMapper;
using PA.Application.DTOs;
using PA.Application.Interfaces.Repositories;
using PA.Application.Interfaces.Services;
using PA.Domain.Entities;

namespace PA.Application.Services;

public class EventoService : IEventoService
{
    private readonly IEventoRepository _eventoRepository;
    private readonly IMapper _mapper;

    public EventoService(IEventoRepository eventoRepository, IMapper mapper)
    {
        _eventoRepository = eventoRepository;
        _mapper = mapper;
    }

    public async Task<EventoDto?> GetByIdAsync(Guid id)
    {
        var evento = await _eventoRepository.GetByIdAsync(id);
        return _mapper.Map<EventoDto>(evento);
    }

    public async Task<IEnumerable<EventoDto>> GetUpcomingAsync()
    {
        var eventos = await _eventoRepository.GetUpcomingEventosAsync();
        return _mapper.Map<IEnumerable<EventoDto>>(eventos);
    }

    public async Task<IEnumerable<EventoDto>> GetPastAsync()
    {
        var eventos = await _eventoRepository.GetPastEventosAsync();
        return _mapper.Map<IEnumerable<EventoDto>>(eventos);
    }

    public async Task<IEnumerable<EventoDto>> GetByGrupoAsync(Guid grupoId)
    {
        var eventos = await _eventoRepository.GetByGrupoAsync(grupoId);
        return _mapper.Map<IEnumerable<EventoDto>>(eventos);
    }

    public async Task<IEnumerable<EventoDto>> GetByPastoralAsync(Guid pastoralId)
    {
        var eventos = await _eventoRepository.GetByPastoralAsync(pastoralId);
        return _mapper.Map<IEnumerable<EventoDto>>(eventos);
    }

    public async Task<EventoDto> CreateAsync(CreateEventoDto dto, Guid userId)
    {
        var evento = new Evento(
            dto.Title,
            dto.Description,
            dto.EventDate,
            userId,
            dto.Location,
            dto.ImageUrl,
            dto.MaxParticipants,
            dto.RequireInscription
        );

        var created = await _eventoRepository.AddAsync(evento);
        return _mapper.Map<EventoDto>(created);
    }

    public async Task UpdateAsync(Guid id, UpdateEventoDto dto)
    {
        var evento = await _eventoRepository.GetByIdAsync(id);
        if (evento == null)
            throw new KeyNotFoundException("Evento não encontrado");

        evento.UpdateInfo(dto.Title, dto.Description, dto.EventDate, dto.Location);
        await _eventoRepository.UpdateAsync(evento);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _eventoRepository.DeleteAsync(id);
    }
}
