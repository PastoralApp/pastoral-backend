using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PA.Application.DTOs;
using PA.Application.Interfaces.Repositories;
using PA.Domain.Entities;
using PA.API.Filters;

namespace PA.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class IgrejasController : ControllerBase
{
    private readonly IIgrejaRepository _igrejaRepository;

    public IgrejasController(IIgrejaRepository igrejaRepository)
    {
        _igrejaRepository = igrejaRepository;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll([FromQuery] bool incluirInativas = false)
    {
        var igrejas = await _igrejaRepository.GetAllAsync(incluirInativas);
        var dtos = igrejas.Select(i => new IgrejaDto
        {
            Id = i.Id,
            Nome = i.Nome,
            Endereco = i.Endereco,
            Telefone = i.Telefone,
            ImagemUrl = i.ImagemUrl,
            IsAtiva = i.IsAtiva,
            HorariosMissas = i.HorariosMissas.Select(h => new HorarioMissaDto
            {
                Id = h.Id,
                IgrejaId = h.IgrejaId,
                DiaSemana = h.DiaSemana,
                DiaSemanaTexto = h.DiaSemana.ToString(),
                Horario = h.Horario,
                HorarioTexto = h.Horario.ToString(@"hh\:mm"),
                Celebrante = h.Celebrante,
                Observacao = h.Observacao,
                IsAtivo = h.IsAtivo
            }).ToList()
        });

        return Ok(dtos);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id)
    {
        var igreja = await _igrejaRepository.GetByIdAsync(id);
        if (igreja == null)
            return NotFound();

        var dto = new IgrejaDto
        {
            Id = igreja.Id,
            Nome = igreja.Nome,
            Endereco = igreja.Endereco,
            Telefone = igreja.Telefone,
            ImagemUrl = igreja.ImagemUrl,
            IsAtiva = igreja.IsAtiva,
            HorariosMissas = igreja.HorariosMissas.Select(h => new HorarioMissaDto
            {
                Id = h.Id,
                IgrejaId = h.IgrejaId,
                DiaSemana = h.DiaSemana,
                DiaSemanaTexto = h.DiaSemana.ToString(),
                Horario = h.Horario,
                HorarioTexto = h.Horario.ToString(@"hh\:mm"),
                Celebrante = h.Celebrante,
                Observacao = h.Observacao,
                IsAtivo = h.IsAtivo
            }).ToList()
        };

        return Ok(dto);
    }

    [HttpPost]
    [ServiceFilter(typeof(AuthorizationFilter))]
    public async Task<IActionResult> Create([FromBody] CreateIgrejaDto dto)
    {
        var igreja = new Igreja(dto.Nome, dto.Endereco, dto.Telefone, dto.ImagemUrl);
        var created = await _igrejaRepository.AddAsync(igreja);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, new IgrejaDto
        {
            Id = created.Id,
            Nome = created.Nome,
            Endereco = created.Endereco,
            Telefone = created.Telefone,
            ImagemUrl = created.ImagemUrl,
            IsAtiva = created.IsAtiva
        });
    }

    [HttpPut("{id}")]
    [ServiceFilter(typeof(AuthorizationFilter))]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateIgrejaDto dto)
    {
        var igreja = await _igrejaRepository.GetByIdAsync(id);
        if (igreja == null)
            return NotFound();

        igreja.Atualizar(dto.Nome, dto.Endereco, dto.Telefone, dto.ImagemUrl);
        await _igrejaRepository.UpdateAsync(igreja);

        return NoContent();
    }

    [HttpDelete("{id}")]
    [ServiceFilter(typeof(AuthorizationFilter))]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _igrejaRepository.DeleteAsync(id);
        return NoContent();
    }

    [HttpPatch("{id}/desativar")]
    [ServiceFilter(typeof(AuthorizationFilter))]
    public async Task<IActionResult> Desativar(Guid id)
    {
        var igreja = await _igrejaRepository.GetByIdAsync(id);
        if (igreja == null)
            return NotFound();

        igreja.Desativar();
        await _igrejaRepository.UpdateAsync(igreja);

        return NoContent();
    }

    [HttpPatch("{id}/ativar")]
    [ServiceFilter(typeof(AuthorizationFilter))]
    public async Task<IActionResult> Ativar(Guid id)
    {
        var igreja = await _igrejaRepository.GetByIdAsync(id);
        if (igreja == null)
            return NotFound();

        igreja.Ativar();
        await _igrejaRepository.UpdateAsync(igreja);

        return NoContent();
    }
}
