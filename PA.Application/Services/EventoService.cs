using AutoMapper;
using PA.Application.DTOs;
using PA.Application.Interfaces.Repositories;
using PA.Application.Interfaces.Services;
using PA.Domain.Entities;
using PA.Domain.Enums;

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

    public async Task<EventoDto?> GetByIdAsync(Guid id, Guid? userId = null)
    {
        var evento = await _eventoRepository.GetByIdAsync(id);
        var dto = _mapper.Map<EventoDto>(evento);
        
        if (dto != null && userId.HasValue)
        {
            dto.UsuarioParticipando = evento?.Participantes.Any(p => p.UserId == userId.Value && p.Confirmado) ?? false;
        }
        
        return dto;
    }

    public async Task<IEnumerable<EventoDto>> GetUpcomingAsync(Guid? userId = null)
    {
        var eventos = await _eventoRepository.GetUpcomingEventosAsync();
        var dtos = _mapper.Map<List<EventoDto>>(eventos);
        
        if (userId.HasValue)
        {
            foreach (var dto in dtos)
            {
                var evento = eventos.FirstOrDefault(e => e.Id == dto.Id);
                dto.UsuarioParticipando = evento?.Participantes.Any(p => p.UserId == userId.Value && p.Confirmado) ?? false;
            }
        }
        
        return dtos;
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

    public async Task<IEnumerable<EventoDto>> GetByTypeAsync(EventoType type)
    {
        var eventos = await _eventoRepository.GetByTypeAsync(type);
        return _mapper.Map<IEnumerable<EventoDto>>(eventos);
    }

    public async Task<EventoDto> CreateAsync(CreateEventoDto dto, Guid userId)
    {
        var evento = new Evento(
            dto.Title,
            dto.Description,
            dto.EventDate,
            userId,
            dto.Type,
            dto.Location,
            dto.ImageUrl,
            dto.BannerUrl,
            dto.MaxParticipants,
            dto.RequireInscription,
            dto.EventEndDate,
            dto.LinkInscricao,
            dto.Preco,
            dto.DataLimiteInscricao,
            dto.GrupoId,
            dto.ResponsavelUserId,
            dto.Cor
        );

        var created = await _eventoRepository.AddAsync(evento);
        return _mapper.Map<EventoDto>(created);
    }

    public async Task UpdateAsync(Guid id, UpdateEventoDto dto)
    {
        var evento = await _eventoRepository.GetByIdAsync(id);
        if (evento == null)
            throw new KeyNotFoundException("Evento não encontrado");

        evento.UpdateInfo(dto.Title, dto.Description, dto.EventDate, dto.Location, dto.EventEndDate);
        evento.UpdateType(dto.Type);
        evento.UpdateCapacity(dto.MaxParticipants);
        evento.UpdateInscricaoInfo(dto.LinkInscricao, dto.Preco, dto.DataLimiteInscricao);
        evento.UpdateGrupo(dto.GrupoId);
        evento.UpdateResponsavel(dto.ResponsavelUserId);
        evento.UpdateCor(dto.Cor);
        
        if (!string.IsNullOrEmpty(dto.ImageUrl))
            evento.UpdateImage(dto.ImageUrl);
            
        if (!string.IsNullOrEmpty(dto.BannerUrl))
            evento.UpdateBanner(dto.BannerUrl);

        await _eventoRepository.UpdateAsync(evento);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _eventoRepository.DeleteAsync(id);
    }

    public async Task<bool> ParticiparAsync(Guid eventoId, Guid userId)
    {
        var evento = await _eventoRepository.GetByIdAsync(eventoId);
        if (evento == null)
            throw new KeyNotFoundException("Evento não encontrado");

        var jaParticipando = evento.Participantes.Any(p => p.UserId == userId && p.Confirmado);
        if (jaParticipando)
            return false;

        if (!evento.VagasDisponiveis)
            throw new InvalidOperationException("Não há vagas disponíveis");

        var participante = new EventoParticipante(eventoId, userId);
        evento.AdicionarParticipante(participante);
        await _eventoRepository.UpdateAsync(evento);
        
        return true;
    }

    public async Task<bool> CancelarParticipacaoAsync(Guid eventoId, Guid userId)
    {
        var evento = await _eventoRepository.GetByIdAsync(eventoId);
        if (evento == null)
            throw new KeyNotFoundException("Evento não encontrado");

        var participante = evento.Participantes.FirstOrDefault(p => p.UserId == userId);
        if (participante == null)
            return false;

        participante.Cancelar();
        await _eventoRepository.UpdateAsync(evento);
        
        return true;
    }

    public async Task<IEnumerable<EventoParticipanteDto>> GetParticipantesAsync(Guid eventoId)
    {
        var evento = await _eventoRepository.GetByIdWithParticipantesAsync(eventoId);
        if (evento == null)
            throw new KeyNotFoundException("Evento não encontrado");

        return _mapper.Map<IEnumerable<EventoParticipanteDto>>(evento.Participantes.Where(p => p.Confirmado));
    }
}
