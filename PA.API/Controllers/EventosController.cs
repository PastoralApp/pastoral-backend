using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PA.Application.DTOs;
using PA.Application.Interfaces.Services;
using System.Security.Claims;

namespace PA.API.Controllers;

/// <summary>
/// Controller de eventos
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EventosController : ControllerBase
{
    private readonly IEventoService _eventoService;

    public EventosController(IEventoService eventoService)
    {
        _eventoService = eventoService;
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
}
