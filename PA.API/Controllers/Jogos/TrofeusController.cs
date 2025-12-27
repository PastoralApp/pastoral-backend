using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PA.Application.DTOs;
using PA.Application.Interfaces.Services;
using PA.API.Filters;

namespace PA.API.Controllers.Jogos;

[ApiController]
[Route("api/[controller]")]
public class TrofeusController : ControllerBase
{
    private readonly ITrofeuService _trofeuService;

    public TrofeusController(ITrofeuService trofeuService)
    {
        _trofeuService = trofeuService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var trofeus = await _trofeuService.GetAllAsync();
        return Ok(trofeus);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id)
    {
        var trofeu = await _trofeuService.GetByIdAsync(id);
        if (trofeu == null)
            return NotFound();
        return Ok(trofeu);
    }

    [HttpGet("jogo/{jogoId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByJogo(Guid jogoId)
    {
        var trofeus = await _trofeuService.GetByJogoIdAsync(jogoId);
        return Ok(trofeus);
    }

    [HttpGet("grupo/{grupoId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByGrupo(Guid grupoId)
    {
        var trofeus = await _trofeuService.GetByGrupoIdAsync(grupoId);
        return Ok(trofeus);
    }

    [HttpGet("ano/{ano}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByAno(int ano)
    {
        var trofeus = await _trofeuService.GetByAnoAsync(ano);
        return Ok(trofeus);
    }

    [HttpPost]
    [Authorize(Roles = "Administrador,Coordenador Geral,Coordenador de Jogo")]
    [ServiceFilter(typeof(AuthorizationFilter))]
    public async Task<IActionResult> Create([FromBody] CreateTrofeuDto dto)
    {
        var created = await _trofeuService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrador")]
    [ServiceFilter(typeof(AuthorizationFilter))]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _trofeuService.DeleteAsync(id);
        return NoContent();
    }
}
