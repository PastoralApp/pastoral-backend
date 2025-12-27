using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PA.Application.DTOs;
using PA.Application.Interfaces.Repositories;
using PA.Application.Interfaces.Services;
using PA.Domain.Entities;
using GrupoEntity = PA.Domain.Entities.Grupo;
using PA.Domain.ValueObjects;
using PA.API.Filters;

namespace PA.API.Controllers.Grupo;

[ApiController]
[Route("api/[controller]")]
public class GruposController : ControllerBase
{
    private readonly IGrupoRepository _grupoRepository;
    private readonly IPastoralRepository _pastoralRepository;
    private readonly IUserRepository _userRepository;
    private readonly IGrupoService _grupoService;

    public GruposController(IGrupoRepository grupoRepository, IPastoralRepository pastoralRepository, IUserRepository userRepository, IGrupoService grupoService)
    {
        _grupoRepository = grupoRepository;
        _pastoralRepository = pastoralRepository;
        _userRepository = userRepository;
        _grupoService = grupoService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll([FromQuery] bool incluirInativos = false)
    {
        var grupos = await _grupoRepository.GetAllAsync(incluirInativos);
        var dtos = grupos.Select(g => new GrupoDto
        {
            Id = g.Id,
            Name = g.Name,
            Sigla = g.Sigla,
            Description = g.Description,
            PastoralId = g.PastoralId,
            PastoralName = g.Pastoral?.Name,
            PastoralSigla = g.Pastoral?.Sigla,
            PrimaryColor = g.Theme.PrimaryColor,
            SecondaryColor = g.Theme.SecondaryColor,
            LogoUrl = g.LogoUrl,
            IsActive = g.IsActive,
            MembersCount = g.UserGrupos?.Count(ug => ug.IsAtivo) ?? 0
        });

        return Ok(dtos);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id)
    {
        var grupo = await _grupoRepository.GetByIdAsync(id);
        if (grupo == null)
            return NotFound();

        var dto = new GrupoDto
        {
            Id = grupo.Id,
            Name = grupo.Name,
            Sigla = grupo.Sigla,
            Description = grupo.Description,
            PastoralId = grupo.PastoralId,
            PastoralName = grupo.Pastoral?.Name,
            PastoralSigla = grupo.Pastoral?.Sigla,
            PrimaryColor = grupo.Theme.PrimaryColor,
            SecondaryColor = grupo.Theme.SecondaryColor,
            LogoUrl = grupo.LogoUrl,
            IsActive = grupo.IsActive,
            MembersCount = grupo.UserGrupos?.Count(ug => ug.IsAtivo) ?? 0
        };

        return Ok(dto);
    }

    [HttpGet("pastoral/{pastoralId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByPastoral(Guid pastoralId, [FromQuery] bool incluirInativos = false)
    {
        var grupos = await _grupoRepository.GetByPastoralIdAsync(pastoralId, incluirInativos);
        var dtos = grupos.Select(g => new GrupoDto
        {
            Id = g.Id,
            Name = g.Name,
            Sigla = g.Sigla,
            Description = g.Description,
            PastoralId = g.PastoralId,
            PastoralName = g.Pastoral?.Name,
            PastoralSigla = g.Pastoral?.Sigla,
            PrimaryColor = g.Theme.PrimaryColor,
            SecondaryColor = g.Theme.SecondaryColor,
            LogoUrl = g.LogoUrl,
            IsActive = g.IsActive,
            MembersCount = g.UserGrupos?.Count(ug => ug.IsAtivo) ?? 0
        });

        return Ok(dtos);
    }

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    [ServiceFilter(typeof(AuthorizationFilter))]
    public async Task<IActionResult> Create([FromBody] CreateGrupoDto dto)
    {
        var grupo = new GrupoEntity(
            dto.Name,
            dto.Sigla,
            dto.Description,
            dto.PastoralId,
            new ColorTheme(dto.PrimaryColor, dto.SecondaryColor),
            dto.LogoUrl
        );

        var created = await _grupoRepository.AddAsync(grupo);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, new GrupoDto
        {
            Id = created.Id,
            Name = created.Name,
            Sigla = created.Sigla,
            Description = created.Description,
            PastoralId = created.PastoralId,
            PrimaryColor = created.Theme.PrimaryColor,
            SecondaryColor = created.Theme.SecondaryColor,
            LogoUrl = created.LogoUrl,
            IsActive = created.IsActive
        });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Administrador")]
    [ServiceFilter(typeof(AuthorizationFilter))]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateGrupoDto dto)
    {
        var grupo = await _grupoRepository.GetByIdAsync(id);
        if (grupo == null)
            return NotFound();

        grupo.UpdateInfo(dto.Name, dto.Sigla, dto.Description, dto.LogoUrl);
        grupo.UpdateTheme(new ColorTheme(dto.PrimaryColor, dto.SecondaryColor));
        await _grupoRepository.UpdateAsync(grupo);

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrador")]
    [ServiceFilter(typeof(AuthorizationFilter))]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _grupoRepository.DeleteAsync(id);
        return NoContent();
    }

    [HttpGet("{id}/membros")]
    [Authorize]
    public async Task<IActionResult> GetMembros(Guid id)
    {
        var users = await _userRepository.GetByGrupoIdAsync(id);
        return Ok(users);
    }

    [HttpPost("{grupoId}/membros/{userId}")]
    [Authorize(Roles = "Administrador,Coordenador Geral,Coordenador de Grupo")]
    public async Task<IActionResult> AddMembro(Guid grupoId, Guid userId)
    {
        try
        {
            await _grupoService.AddMemberAsync(grupoId, userId);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{grupoId}/membros/{userId}")]
    [Authorize(Roles = "Administrador,Coordenador Geral,Coordenador de Grupo")]
    public async Task<IActionResult> RemoveMembro(Guid grupoId, Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return NotFound();

        user.RemoverDoGrupo(grupoId);
        await _userRepository.UpdateAsync(user);

        return NoContent();
    }

    [HttpPost("{grupoId}/entrar")]
    [Authorize]
    public async Task<IActionResult> EntrarNoGrupo(Guid grupoId)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        try
        {
            var grupo = await _grupoRepository.GetByIdAsync(grupoId);
            if (grupo == null)
                return NotFound(new { message = "Grupo não encontrado" });

            if (!grupo.IsActive)
                return BadRequest(new { message = "Este grupo está inativo" });

            await _grupoService.AddMemberAsync(grupoId, userId);
            
            return Ok(new { message = "Você entrou no grupo com sucesso!" });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{grupoId}/sair")]
    [Authorize]
    public async Task<IActionResult> SairDoGrupo(Guid grupoId)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return NotFound("Usuário não encontrado");

        var userGrupo = user.UserGrupos.FirstOrDefault(ug => ug.GrupoId == grupoId && ug.IsAtivo);
        if (userGrupo == null)
            return BadRequest(new { message = "Você não faz parte deste grupo" });

        user.RemoverDoGrupo(grupoId);
        await _userRepository.UpdateAsync(user);

        return Ok(new { message = "Você saiu do grupo com sucesso!" });
    }

    [HttpPost("{grupoId}/silenciar-notificacoes")]
    [Authorize]
    public async Task<IActionResult> SilenciarNotificacoes(Guid grupoId)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return NotFound();

        var userGrupo = user.UserGrupos.FirstOrDefault(ug => ug.GrupoId == grupoId && ug.IsAtivo);
        if (userGrupo == null)
            return NotFound("Usuário não faz parte deste grupo");

        userGrupo.SilenciarNotificacoesDoGrupo();
        await _userRepository.UpdateAsync(user);

        return NoContent();
    }

    [HttpPost("{grupoId}/ativar-notificacoes")]
    [Authorize]
    public async Task<IActionResult> AtivarNotificacoes(Guid grupoId)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return NotFound();

        var userGrupo = user.UserGrupos.FirstOrDefault(ug => ug.GrupoId == grupoId && ug.IsAtivo);
        if (userGrupo == null)
            return NotFound("Usuário não faz parte deste grupo");

        userGrupo.AtivarNotificacoesDoGrupo();
        await _userRepository.UpdateAsync(user);

        return NoContent();
    }

    [HttpPatch("{id}/desativar")]
    [Authorize(Roles = "Administrador")]
    [ServiceFilter(typeof(AuthorizationFilter))]
    public async Task<IActionResult> Desativar(Guid id)
    {
        var grupo = await _grupoRepository.GetByIdAsync(id);
        if (grupo == null)
            return NotFound();

        grupo.Deactivate();
        await _grupoRepository.UpdateAsync(grupo);
        return NoContent();
    }

    [HttpPatch("{id}/ativar")]
    [Authorize(Roles = "Administrador")]
    [ServiceFilter(typeof(AuthorizationFilter))]
    public async Task<IActionResult> Ativar(Guid id)
    {
        var grupo = await _grupoRepository.GetByIdAsync(id);
        if (grupo == null)
            return NotFound();

        grupo.Activate();
        await _grupoRepository.UpdateAsync(grupo);
        return NoContent();
    }
}
