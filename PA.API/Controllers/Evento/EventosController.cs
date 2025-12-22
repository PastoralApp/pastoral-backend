using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PA.Application.DTOs;
using PA.Application.Interfaces.Services;
using PA.Application.Interfaces.Repositories;
using PA.Domain.Entities;
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

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var eventos = await _eventoService.GetUpcomingAsync();
        return Ok(eventos);
    }

    [HttpGet("upcoming")]
    public async Task<IActionResult> GetUpcoming()
    {
        var eventos = await _eventoService.GetUpcomingAsync();
        return Ok(eventos);
    }

    [HttpGet("past")]
    public async Task<IActionResult> GetPast()
    {
        var eventos = await _eventoService.GetPastAsync();
        return Ok(eventos);
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
        var evento = await _eventoService.GetByIdAsync(id);
        
        if (evento == null)
            return NotFound();

        return Ok(evento);
    }

    [HttpPost]
    [Authorize(Roles = "CoordenadorGrupo,CoordenadorGeral,Admin")]
    public async Task<IActionResult> Create([FromBody] CreateEventoDto dto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var evento = await _eventoService.CreateAsync(dto, userId);
        
        return CreatedAtAction(nameof(GetById), new { id = evento.Id }, evento);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "CoordenadorGrupo,CoordenadorGeral,Admin")]
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
    [Authorize(Roles = "CoordenadorGeral,Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _eventoService.DeleteAsync(id);
        return NoContent();
    }

    [HttpPost("{id:guid}/save")]
    public async Task<IActionResult> SaveEvento(Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
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
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var savedEventos = await _eventoRepository.GetSavedEventosByUserAsync(userId);
        return Ok(savedEventos);
    }
}
