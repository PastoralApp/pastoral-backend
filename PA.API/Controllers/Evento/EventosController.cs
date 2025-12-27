using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PA.Application.DTOs;
using PA.Application.Interfaces.Services;
using PA.Application.Interfaces.Repositories;
using PA.Domain.Entities;
using PA.Domain.Enums;
using System.Security.Claims;

namespace PA.API.Controllers.Evento;


[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EventosController : ControllerBase
{
    private readonly IEventoService _eventoService;
    private readonly IEventoRepository _eventoRepository;

    public EventosController(IEventoService eventoService, IEventoRepository eventoRepository)
    {
        _eventoService = eventoService;
        _eventoRepository = eventoRepository;
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = GetUserId();
        var eventos = await _eventoService.GetUpcomingAsync(userId);
        return Ok(eventos);
    }

    [HttpGet("upcoming")]
    public async Task<IActionResult> GetUpcoming()
    {
        var userId = GetUserId();
        var eventos = await _eventoService.GetUpcomingAsync(userId);
        return Ok(eventos);
    }

    [HttpGet("past")]
    public async Task<IActionResult> GetPast()
    {
        var eventos = await _eventoService.GetPastAsync();
        return Ok(eventos);
    }

    [HttpGet("type/{type}")]
    public async Task<IActionResult> GetByType(EventoType type)
    {
        var eventos = await _eventoService.GetByTypeAsync(type);
        return Ok(eventos);
    }

    [HttpGet("types")]
    public IActionResult GetEventoTypes()
    {
        var types = Enum.GetValues<EventoType>()
            .Select(t => new { Value = (int)t, Name = t.ToString() })
            .ToList();
        return Ok(types);
    }

    [HttpGet("grupo/{grupoId:guid}")]
    public async Task<IActionResult> GetByGrupo(Guid grupoId)
    {
        var eventos = await _eventoService.GetByGrupoAsync(grupoId);
        return Ok(eventos);
    }

    [HttpGet("pastoral/{pastoralId:guid}")]
    public async Task<IActionResult> GetByPastoral(Guid pastoralId)
    {
        var eventos = await _eventoService.GetByPastoralAsync(pastoralId);
        return Ok(eventos);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var userId = GetUserId();
        var evento = await _eventoService.GetByIdAsync(id, userId);
        
        if (evento == null)
            return NotFound();

        return Ok(evento);
    }

    [HttpPost]
    [Authorize(Roles = "Coordenador de Grupo,Coordenador Geral,Administrador")]
    public async Task<IActionResult> Create([FromBody] CreateEventoDto dto)
    {
        var userId = GetUserId();
        var evento = await _eventoService.CreateAsync(dto, userId);
        
        return CreatedAtAction(nameof(GetById), new { id = evento.Id }, evento);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Coordenador de Grupo,Coordenador Geral,Administrador")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEventoDto dto)
    {
        try
        {
            await _eventoService.UpdateAsync(id, dto);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Coordenador Geral,Administrador")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _eventoService.DeleteAsync(id);
        return NoContent();
    }

    [HttpPost("{id:guid}/participar")]
    public async Task<IActionResult> Participar(Guid id)
    {
        try
        {
            var userId = GetUserId();
            var resultado = await _eventoService.ParticiparAsync(id, userId);
            return Ok(new { participando = resultado });
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:guid}/participar")]
    public async Task<IActionResult> CancelarParticipacao(Guid id)
    {
        try
        {
            var userId = GetUserId();
            var resultado = await _eventoService.CancelarParticipacaoAsync(id, userId);
            return Ok(new { participando = !resultado });
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("{id:guid}/participantes")]
    public async Task<IActionResult> GetParticipantes(Guid id)
    {
        try
        {
            var participantes = await _eventoService.GetParticipantesAsync(id);
            return Ok(participantes);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost("{id:guid}/save")]
    public async Task<IActionResult> SaveEvento(Guid id)
    {
        var userId = GetUserId();
        var evento = await _eventoRepository.GetByIdAsync(id);
        
        if (evento == null)
            return NotFound();

        var saved = await _eventoRepository.GetEventoSalvoAsync(id, userId);

        if (saved != null)
        {
            await _eventoRepository.RemoveEventoSalvoAsync(saved);
            return Ok(new { saved = false });
        }
        else
        {
            var eventoSalvo = new EventoSalvo(id, userId);
            await _eventoRepository.AddEventoSalvoAsync(eventoSalvo);
            return Ok(new { saved = true });
        }
    }

    [HttpGet("saved")]
    public async Task<IActionResult> GetSavedEventos()
    {
        var userId = GetUserId();
        var savedEventos = await _eventoRepository.GetSavedEventosByUserAsync(userId);
        return Ok(savedEventos);
    }
}
