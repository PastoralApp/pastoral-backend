using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PA.Application.DTOs;
using PA.Application.Interfaces.Services;
using PA.API.Filters;

namespace PA.API.Controllers.Grupo;

[ApiController]
[Route("api/[controller]")]
public class GruposController : ControllerBase
{
    private readonly IGrupoService _grupoService;

    public GruposController(IGrupoService grupoService)
    {
        _grupoService = grupoService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var grupos = await _grupoService.GetAllAsync();
        return Ok(grupos);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id)
    {
        var grupo = await _grupoService.GetByIdAsync(id);
        if (grupo == null)
            return NotFound();
        return Ok(grupo);
    }

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    [ServiceFilter(typeof(AuthorizationFilter))]
    public async Task<IActionResult> Create([FromBody] CreateGrupoDto dto)
    {
        var created = await _grupoService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Administrador")]
    [ServiceFilter(typeof(AuthorizationFilter))]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateGrupoDto dto)
    {
        var updated = await _grupoService.UpdateAsync(id, dto);
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrador")]
    [ServiceFilter(typeof(AuthorizationFilter))]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _grupoService.DeleteAsync(id);
        return NoContent();
    }

    [HttpPost("{grupoId}/participantes")]
    [Authorize(Roles = "Administrador,Coordenador Geral")]
    public async Task<IActionResult> AdicionarParticipante(Guid grupoId, [FromBody] AdicionarParticipanteGrupoDto dto)
    {
        dto.GrupoId = grupoId;
        var success = await _grupoService.AdicionarParticipanteAsync(dto);
        return success ? Ok() : BadRequest("Não foi possível adicionar participante");
    }

    [HttpDelete("{grupoId}/participantes/{userId}")]
    [Authorize(Roles = "Administrador,Coordenador Geral")]
    public async Task<IActionResult> RemoverParticipante(Guid grupoId, Guid userId)
    {
        var dto = new RemoverParticipanteGrupoDto { GrupoId = grupoId, UserId = userId };
        var success = await _grupoService.RemoverParticipanteAsync(dto);
        return success ? Ok() : BadRequest("Não foi possível remover participante");
    }

    [HttpGet("{id}/medalhas")]
    [AllowAnonymous]
    public async Task<IActionResult> GetMedalhas(Guid id)
    {
        var medalhas = await _grupoService.GetMedalhasAsync(id);
        return Ok(medalhas);
    }

    [HttpGet("{id}/trofeus")]
    [AllowAnonymous]
    public async Task<IActionResult> GetTrofeus(Guid id)
    {
        var trofeus = await _grupoService.GetTrofeusAsync(id);
        return Ok(trofeus);
    }
}
