using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PA.Application.DTOs;
using PA.Application.Interfaces.Services;

namespace PA.API.Controllers.Jogos;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GrupoJogosController : ControllerBase
{
    private readonly IGrupoService _grupoService;

    public GrupoJogosController(IGrupoService grupoService)
    {
        _grupoService = grupoService;
    }


    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<GrupoDto>>> GetAll()
    {
        var grupos = await _grupoService.GetAllAsync();
        return Ok(grupos);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<GrupoDto>> GetById(Guid id)
    {
        var grupo = await _grupoService.GetByIdAsync(id);
        if (grupo == null)
            return NotFound(new { message = "Grupo não encontrado" });

        return Ok(grupo);
    }

    [HttpGet("{id}/participantes")]
    [AllowAnonymous]
    public async Task<ActionResult<GrupoDto>> GetByIdWithParticipantes(Guid id)
    {
        var grupo = await _grupoService.GetByIdWithParticipantesAsync(id);
        if (grupo == null)
            return NotFound(new { message = "Grupo não encontrado" });

        return Ok(grupo);
    }

    [HttpGet("{id}/conquistas")]
    [AllowAnonymous]
    public async Task<ActionResult<GrupoDto>> GetByIdWithConquistas(Guid id)
    {
        var grupo = await _grupoService.GetByIdWithConquistasAsync(id);
        if (grupo == null)
            return NotFound(new { message = "Grupo não encontrado" });

        return Ok(grupo);
    }

    [HttpGet("{id}/medalhas")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<MedalhaDto>>> GetMedalhas(Guid id)
    {
        var medalhas = await _grupoService.GetMedalhasAsync(id);
        return Ok(medalhas);
    }

    [HttpGet("{id}/trofeus")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<TrofeuDto>>> GetTrofeus(Guid id)
    {
        var trofeus = await _grupoService.GetTrofeusAsync(id);
        return Ok(trofeus);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,CoordenadorGeral,CoordenadorDeJogo")]
    public async Task<ActionResult<GrupoDto>> Create([FromBody] CreateGrupoDto dto)
    {
        try
        {
            var grupo = await _grupoService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = grupo.Id }, grupo);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,CoordenadorGeral,CoordenadorDeJogo")]
    public async Task<ActionResult<GrupoDto>> Update(Guid id, [FromBody] UpdateGrupoDto dto)
    {
        try
        {
            var grupo = await _grupoService.UpdateAsync(id, dto);
            return Ok(grupo);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("participante")]
    [Authorize(Roles = "Admin,CoordenadorGeral,CoordenadorDeJogo")]
    public async Task<ActionResult> AdicionarParticipante([FromBody] AdicionarParticipanteGrupoDto dto)
    {
        try
        {
            var sucesso = await _grupoService.AdicionarParticipanteAsync(dto);
            if (!sucesso)
                return BadRequest(new { message = "Não foi possível adicionar o participante" });

            return Ok(new { message = "Participante adicionado com sucesso" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("participante")]
    [Authorize(Roles = "Admin,CoordenadorGeral,CoordenadorDeJogo")]
    public async Task<ActionResult> RemoverParticipante([FromBody] RemoverParticipanteGrupoDto dto)
    {
        try
        {
            var sucesso = await _grupoService.RemoverParticipanteAsync(dto);
            if (!sucesso)
                return BadRequest(new { message = "Não foi possível remover o participante" });

            return Ok(new { message = "Participante removido com sucesso" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Delete(Guid id)
    {
        try
        {
            await _grupoService.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
