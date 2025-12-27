using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PA.API.Filters;
using PA.Application.DTOs;
using PA.Application.Interfaces.Services;
using PA.Domain.Enums;
using System.Security.Claims;

namespace PA.API.Controllers.Jogos;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OlimpiadasController : ControllerBase
{
    private readonly IOlimpiadasService _olimpiadasService;

    public OlimpiadasController(IOlimpiadasService olimpiadasService)
    {
        _olimpiadasService = olimpiadasService;
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<OlimpiadasDto>>> GetAll()
    {
        var olimpiadas = await _olimpiadasService.GetAllAsync();
        return Ok(olimpiadas);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<OlimpiadasDto>> GetById(Guid id)
    {
        var olimpiadas = await _olimpiadasService.GetByIdAsync(id);
        if (olimpiadas == null)
            return NotFound(new { message = "Olimpíada não encontrada" });

        return Ok(olimpiadas);
    }

    [HttpGet("{id}/detalhes")]
    [AllowAnonymous]
    public async Task<ActionResult<OlimpiadasDto>> GetByIdWithDetails(Guid id)
    {
        var olimpiadas = await _olimpiadasService.GetByIdWithDetailsAsync(id);
        if (olimpiadas == null)
            return NotFound(new { message = "Olimpíada não encontrada" });

        return Ok(olimpiadas);
    }

    [HttpGet("pastoral/{pastoralId}")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<OlimpiadasDto>>> GetByPastoralId(Guid pastoralId)
    {
        var olimpiadas = await _olimpiadasService.GetByPastoralIdAsync(pastoralId);
        return Ok(olimpiadas);
    }

    [HttpGet("ano/{ano}")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<OlimpiadasDto>>> GetByAno(int ano)
    {
        var olimpiadas = await _olimpiadasService.GetByAnoAsync(ano);
        return Ok(olimpiadas);
    }

    [HttpGet("status/{status}")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<OlimpiadasDto>>> GetByStatus(StatusJogo status)
    {
        var olimpiadas = await _olimpiadasService.GetByStatusAsync(status);
        return Ok(olimpiadas);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,CoordenadorGeral,CoordenadorDeJogo")]
    public async Task<ActionResult<OlimpiadasDto>> Create([FromBody] CreateOlimpiadasDto dto)
    {
        try
        {
            var userId = GetUserId();
            var olimpiadas = await _olimpiadasService.CreateAsync(dto, userId);
            return CreatedAtAction(nameof(GetById), new { id = olimpiadas.Id }, olimpiadas);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,CoordenadorGeral,CoordenadorDeJogo")]
    public async Task<ActionResult<OlimpiadasDto>> Update(Guid id, [FromBody] UpdateOlimpiadasDto dto)
    {
        try
        {
            var userId = GetUserId();
            var olimpiadas = await _olimpiadasService.UpdateAsync(id, dto, userId);
            return Ok(olimpiadas);
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

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Delete(Guid id)
    {
        try
        {
            await _olimpiadasService.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    
    [HttpPost("{olimpiadasId}/grupos/{grupoId}")]
    [Authorize(Roles = "Admin,CoordenadorGeral,CoordenadorDeJogo")]
    public async Task<ActionResult> InscreverGrupo(Guid olimpiadasId, Guid grupoId)
    {
        var sucesso = await _olimpiadasService.InscreverGrupoAsync(olimpiadasId, grupoId);
        if (!sucesso)
            return BadRequest(new { message = "Não foi possível inscrever o grupo" });

        return Ok(new { message = "Grupo inscrito com sucesso" });
    }

    [HttpDelete("{olimpiadasId}/grupos/{grupoId}")]
    [Authorize(Roles = "Admin,CoordenadorGeral,CoordenadorDeJogo")]
    public async Task<ActionResult> RemoverGrupo(Guid olimpiadasId, Guid grupoId)
    {
        var sucesso = await _olimpiadasService.RemoverGrupoAsync(olimpiadasId, grupoId);
        if (!sucesso)
            return BadRequest(new { message = "Não foi possível remover o grupo" });

        return Ok(new { message = "Grupo removido com sucesso" });
    }

    [HttpGet("{id}/ranking")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<RankingOlimpiadasDto>>> GetRanking(Guid id)
    {
        var ranking = await _olimpiadasService.GetRankingAsync(id);
        return Ok(ranking);
    }

    [HttpPost("{id}/ranking/atualizar")]
    [Authorize(Roles = "Admin,CoordenadorGeral,CoordenadorDeJogo")]
    public async Task<ActionResult> AtualizarRanking(Guid id)
    {
        try
        {
            await _olimpiadasService.AtualizarRankingAsync(id);
            return Ok(new { message = "Ranking atualizado com sucesso" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
