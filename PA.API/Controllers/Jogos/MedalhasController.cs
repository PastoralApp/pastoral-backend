using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PA.Application.DTOs;
using PA.Application.Interfaces.Services;
using PA.Domain.Enums;
using PA.API.Filters;

namespace PA.API.Controllers.Jogos;

[ApiController]
[Route("api/[controller]")]
public class MedalhasController : ControllerBase
{
    private readonly IMedalhaService _medalhaService;

    public MedalhasController(IMedalhaService medalhaService)
    {
        _medalhaService = medalhaService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var medalhas = await _medalhaService.GetAllAsync();
        return Ok(medalhas);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id)
    {
        var medalha = await _medalhaService.GetByIdAsync(id);
        if (medalha == null)
            return NotFound();
        return Ok(medalha);
    }

    [HttpGet("jogo/{jogoId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByJogo(Guid jogoId)
    {
        var medalhas = await _medalhaService.GetByJogoIdAsync(jogoId);
        return Ok(medalhas);
    }

    [HttpGet("grupo/{grupoId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByGrupo(Guid grupoId)
    {
        var medalhas = await _medalhaService.GetByGrupoIdAsync(grupoId);
        return Ok(medalhas);
    }

    [HttpGet("participante/{participanteId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByParticipante(Guid participanteId)
    {
        var medalhas = await _medalhaService.GetByParticipanteIdAsync(participanteId);
        return Ok(medalhas);
    }

    [HttpGet("ano/{ano}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByAno(int ano)
    {
        var medalhas = await _medalhaService.GetByAnoAsync(ano);
        return Ok(medalhas);
    }

    [HttpGet("tipo/{tipo}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByTipo(TipoMedalha tipo)
    {
        var medalhas = await _medalhaService.GetByTipoAsync(tipo);
        return Ok(medalhas);
    }

    [HttpPost]
    [Authorize(Roles = "Administrador,Coordenador Geral,Coordenador de Jogo")]
    [ServiceFilter(typeof(AuthorizationFilter))]
    public async Task<IActionResult> Create([FromBody] CreateMedalhaDto dto)
    {
        var created = await _medalhaService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrador")]
    [ServiceFilter(typeof(AuthorizationFilter))]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _medalhaService.DeleteAsync(id);
        return NoContent();
    }
}
