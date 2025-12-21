using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PA.Application.DTOs;
using PA.Application.Interfaces.Repositories;
using PA.Domain.Entities;
using PA.API.Filters;

namespace PA.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HorariosMissasController : ControllerBase
{
    private readonly IHorarioMissaRepository _horarioMissaRepository;

    public HorariosMissasController(IHorarioMissaRepository horarioMissaRepository)
    {
        _horarioMissaRepository = horarioMissaRepository;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll([FromQuery] bool incluirInativos = false)
    {
        var horarios = await _horarioMissaRepository.GetAllAsync(incluirInativos);
        var dtos = horarios.Select(h => new HorarioMissaDto
        {
            Id = h.Id,
            IgrejaId = h.IgrejaId,
            IgrejaNome = h.Igreja?.Nome,
            DiaSemana = h.DiaSemana,
            DiaSemanaTexto = h.DiaSemana.ToString(),
            Horario = h.Horario,
            HorarioTexto = h.Horario.ToString(@"hh\:mm"),
            Celebrante = h.Celebrante,
            Observacao = h.Observacao,
            IsAtivo = h.IsAtivo
        });

        return Ok(dtos);
    }

    [HttpGet("igreja/{igrejaId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByIgreja(Guid igrejaId, [FromQuery] bool incluirInativos = false)
    {
        var horarios = await _horarioMissaRepository.GetByIgrejaIdAsync(igrejaId, incluirInativos);
        var dtos = horarios.Select(h => new HorarioMissaDto
        {
            Id = h.Id,
            IgrejaId = h.IgrejaId,
            IgrejaNome = h.Igreja?.Nome,
            DiaSemana = h.DiaSemana,
            DiaSemanaTexto = h.DiaSemana.ToString(),
            Horario = h.Horario,
            HorarioTexto = h.Horario.ToString(@"hh\:mm"),
            Celebrante = h.Celebrante,
            Observacao = h.Observacao,
            IsAtivo = h.IsAtivo
        });

        return Ok(dtos);
    }

    [HttpGet("dia/{diaSemana}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByDiaSemana(DayOfWeek diaSemana, [FromQuery] bool incluirInativos = false)
    {
        var horarios = await _horarioMissaRepository.GetByDiaSemanaAsync(diaSemana, incluirInativos);
        var dtos = horarios.Select(h => new HorarioMissaDto
        {
            Id = h.Id,
            IgrejaId = h.IgrejaId,
            IgrejaNome = h.Igreja?.Nome,
            DiaSemana = h.DiaSemana,
            DiaSemanaTexto = h.DiaSemana.ToString(),
            Horario = h.Horario,
            HorarioTexto = h.Horario.ToString(@"hh\:mm"),
            Celebrante = h.Celebrante,
            Observacao = h.Observacao,
            IsAtivo = h.IsAtivo
        });

        return Ok(dtos);
    }

    [HttpPost]
    [Authorize]
    [ServiceFilter(typeof(AuthorizationFilter))]
    public async Task<IActionResult> Create([FromBody] CreateHorarioMissaDto dto)
    {
        var horario = new HorarioMissa(dto.IgrejaId, dto.DiaSemana, dto.Horario, dto.Celebrante, dto.Observacao);
        var created = await _horarioMissaRepository.AddAsync(horario);

        return CreatedAtAction(nameof(GetAll), new { id = created.Id }, new HorarioMissaDto
        {
            Id = created.Id,
            IgrejaId = created.IgrejaId,
            DiaSemana = created.DiaSemana,
            DiaSemanaTexto = created.DiaSemana.ToString(),
            Horario = created.Horario,
            HorarioTexto = created.Horario.ToString(@"hh\:mm"),
            Celebrante = created.Celebrante,
            Observacao = created.Observacao,
            IsAtivo = created.IsAtivo
        });
    }

    [HttpPut("{id}")]
    [Authorize]
    [ServiceFilter(typeof(AuthorizationFilter))]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateHorarioMissaDto dto)
    {
        var horario = await _horarioMissaRepository.GetByIdAsync(id);
        if (horario == null)
            return NotFound();

        horario.Atualizar(dto.DiaSemana, dto.Horario, dto.Celebrante, dto.Observacao);
        await _horarioMissaRepository.UpdateAsync(horario);

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize]
    [ServiceFilter(typeof(AuthorizationFilter))]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _horarioMissaRepository.DeleteAsync(id);
        return NoContent();
    }
}
