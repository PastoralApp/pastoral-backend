using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PA.Application.DTOs;
using PA.Application.Interfaces.Repositories;
using PA.Domain.Entities;
using PA.Domain.Enums;
using PA.Domain.ValueObjects;
using PA.API.Filters;

namespace PA.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PastoraisController : ControllerBase
{
    private readonly IPastoralRepository _pastoralRepository;

    public PastoraisController(IPastoralRepository pastoralRepository)
    {
        _pastoralRepository = pastoralRepository;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll([FromQuery] bool incluirInativas = false)
    {
        var pastorais = await _pastoralRepository.GetAllAsync(incluirInativas);
        var dtos = pastorais.Select(p => new PastoralDto
        {
            Id = p.Id,
            Name = p.Name,
            Sigla = p.Sigla,
            TipoPastoral = p.TipoPastoral.ToString(),
            Type = p.Type.ToString(),
            Description = p.Description,
            PrimaryColor = p.Theme.PrimaryColor,
            SecondaryColor = p.Theme.SecondaryColor,
            LogoUrl = p.LogoUrl,
            IsActive = p.IsActive,
            Grupos = p.Grupos.Where(g => g.IsActive).Select(g => new GrupoDto
            {
                Id = g.Id,
                Name = g.Name,
                Sigla = g.Sigla,
                Description = g.Description,
                PrimaryColor = g.Theme.PrimaryColor,
                SecondaryColor = g.Theme.SecondaryColor,
                LogoUrl = g.LogoUrl,
                IsActive = g.IsActive,
                MembersCount = g.UserGrupos?.Count(ug => ug.IsAtivo) ?? 0
            }).ToList()
        });

        return Ok(dtos);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id)
    {
        var pastoral = await _pastoralRepository.GetByIdAsync(id);
        if (pastoral == null)
            return NotFound();

        var dto = new PastoralDto
        {
            Id = pastoral.Id,
            Name = pastoral.Name,
            Sigla = pastoral.Sigla,
            TipoPastoral = pastoral.TipoPastoral.ToString(),
            Type = pastoral.Type.ToString(),
            Description = pastoral.Description,
            PrimaryColor = pastoral.Theme.PrimaryColor,
            SecondaryColor = pastoral.Theme.SecondaryColor,
            LogoUrl = pastoral.LogoUrl,
            IsActive = pastoral.IsActive,
            Grupos = pastoral.Grupos.Where(g => g.IsActive).Select(g => new GrupoDto
            {
                Id = g.Id,
                Name = g.Name,
                Sigla = g.Sigla,
                Description = g.Description,
                PrimaryColor = g.Theme.PrimaryColor,
                SecondaryColor = g.Theme.SecondaryColor,
                LogoUrl = g.LogoUrl,
                IsActive = g.IsActive,
                MembersCount = g.UserGrupos?.Count(ug => ug.IsAtivo) ?? 0
            }).ToList()
        };

        return Ok(dto);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ServiceFilter(typeof(AuthorizationFilter))]
    public async Task<IActionResult> Create([FromBody] CreatePastoralDto dto)
    {
        var pastoral = new Pastoral(
            dto.Name,
            dto.Sigla,
            Enum.Parse<TipoPastoral>(dto.TipoPastoral),
            Enum.Parse<PastoralType>(dto.Type),
            new ColorTheme(dto.PrimaryColor, dto.SecondaryColor),
            dto.Description,
            dto.LogoUrl
        );

        var created = await _pastoralRepository.AddAsync(pastoral);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, new PastoralDto
        {
            Id = created.Id,
            Name = created.Name,
            Sigla = created.Sigla,
            TipoPastoral = created.TipoPastoral.ToString(),
            Type = created.Type.ToString(),
            Description = created.Description,
            PrimaryColor = created.Theme.PrimaryColor,
            SecondaryColor = created.Theme.SecondaryColor,
            LogoUrl = created.LogoUrl,
            IsActive = created.IsActive
        });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ServiceFilter(typeof(AuthorizationFilter))]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreatePastoralDto dto)
    {
        var pastoral = await _pastoralRepository.GetByIdAsync(id);
        if (pastoral == null)
            return NotFound();

        pastoral.UpdateInfo(dto.Name, dto.Sigla, dto.Description, dto.LogoUrl);
        pastoral.UpdateTheme(new ColorTheme(dto.PrimaryColor, dto.SecondaryColor));
        await _pastoralRepository.UpdateAsync(pastoral);

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ServiceFilter(typeof(AuthorizationFilter))]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _pastoralRepository.DeleteAsync(id);
        return NoContent();
    }

    [HttpPatch("{id}/desativar")]
    [Authorize(Roles = "Admin")]
    [ServiceFilter(typeof(AuthorizationFilter))]
    public async Task<IActionResult> Desativar(Guid id)
    {
        var pastoral = await _pastoralRepository.GetByIdAsync(id);
        if (pastoral == null)
            return NotFound();

        pastoral.Deactivate();
        await _pastoralRepository.UpdateAsync(pastoral);
        return NoContent();
    }

    [HttpPatch("{id}/ativar")]
    [Authorize(Roles = "Admin")]
    [ServiceFilter(typeof(AuthorizationFilter))]
    public async Task<IActionResult> Ativar(Guid id)
    {
        var pastoral = await _pastoralRepository.GetByIdAsync(id);
        if (pastoral == null)
            return NotFound();

        pastoral.Activate();
        await _pastoralRepository.UpdateAsync(pastoral);
        return NoContent();
    }
}
