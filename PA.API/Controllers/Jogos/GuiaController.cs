using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PA.Application.DTOs;
using PA.Application.Interfaces.Services;
using PA.Domain.Enums;
using System.Security.Claims;

namespace PA.API.Controllers.Jogos;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GuiaController : ControllerBase
{
    private readonly IGuiaService _guiaService;

    public GuiaController(IGuiaService guiaService)
    {
        _guiaService = guiaService;
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<GuiaDto>>> GetAll()
    {
        var guias = await _guiaService.GetAllAsync();
        return Ok(guias);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<GuiaDto>> GetById(Guid id)
    {
        var guia = await _guiaService.GetByIdAsync(id);
        if (guia == null)
            return NotFound(new { message = "Guia não encontrada" });

        return Ok(guia);
    }

    [HttpGet("{id}/detalhes")]
    [AllowAnonymous]
    public async Task<ActionResult<GuiaDto>> GetByIdWithDetails(Guid id)
    {
        var guia = await _guiaService.GetByIdWithDetailsAsync(id);
        if (guia == null)
            return NotFound(new { message = "Guia não encontrada" });

        return Ok(guia);
    }


    [HttpPost]
    [Authorize(Roles = "Admin,CoordenadorGeral,CoordenadorDeJogo")]
    public async Task<ActionResult<GuiaDto>> Create([FromBody] CreateGuiaDto dto)
    {
        try
        {
            var userId = GetUserId();
            var guia = await _guiaService.CreateAsync(dto, userId);
            return CreatedAtAction(nameof(GetById), new { id = guia.Id }, guia);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,CoordenadorGeral,CoordenadorDeJogo")]
    public async Task<ActionResult<GuiaDto>> Update(Guid id, [FromBody] UpdateGuiaDto dto)
    {
        try
        {
            var userId = GetUserId();
            var guia = await _guiaService.UpdateAsync(id, dto, userId);
            return Ok(guia);
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
            await _guiaService.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id}/ranking")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<RankingGuiaDto>>> GetRanking(Guid id)
    {
        var ranking = await _guiaService.GetRankingAsync(id);
        return Ok(ranking);
    }

    [HttpPost("{id}/ranking/atualizar")]
    [Authorize(Roles = "Admin,CoordenadorGeral,CoordenadorDeJogo")]
    public async Task<ActionResult> AtualizarRanking(Guid id)
    {
        try
        {
            await _guiaService.AtualizarRankingAsync(id);
            return Ok(new { message = "Ranking atualizado com sucesso" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
