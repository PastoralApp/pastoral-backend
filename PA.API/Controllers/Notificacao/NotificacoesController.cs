using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PA.Application.DTOs;
using PA.Application.Interfaces.Repositories;
using PA.Domain.Entities;
using PA.API.Filters;
using PA.Application.Interfaces.Services;
using AutoMapper;
using System.Security.Claims;

namespace PA.API.Controllers.Notificacao;

[ApiController]
[Route("api/[controller]")]
public class NotificacoesController : ControllerBase
{
    private readonly INotificacaoRepository _notificacaoRepository;
    private readonly IGrupoRepository _grupoRepository;
    private readonly IUserRepository _userRepository;
    private readonly INotificacaoService _notificacaoService;
    private readonly IMapper _mapper;

    public NotificacoesController(
        INotificacaoRepository notificacaoRepository,
        IGrupoRepository grupoRepository,
        IUserRepository userRepository,
        INotificacaoService notificacaoService,
        IMapper mapper)
    {
        _notificacaoRepository = notificacaoRepository;
        _grupoRepository = grupoRepository;
        _userRepository = userRepository;
        _notificacaoService = notificacaoService;
        _mapper = mapper;
    }

    [HttpGet("minhas")]
    [Authorize]
    public async Task<IActionResult> GetMinhasNotificacoes()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var notificacoes = await _notificacaoRepository.GetByUserIdAsync(userId);

        var dtos = notificacoes.Select(n => new NotificacaoDto
        {
            Id = n.Id,
            Titulo = n.Titulo,
            Mensagem = n.Mensagem,
            GrupoId = n.GrupoId,
            GrupoNome = n.Grupo?.Name,
            GrupoSigla = n.Grupo?.Sigla,
            RemetenteId = n.RemetenteId,
            RemetenteNome = n.Remetente.Name,
            DataEnvio = n.DataEnvio,
            IsAtiva = n.IsAtiva,
            IsGeral = n.IsGeral,
            Lida = n.Leituras.Any(l => l.UserId == userId),
            DataLeitura = n.Leituras.FirstOrDefault(l => l.UserId == userId)?.DataLeitura
        });

        return Ok(dtos);
    }

    [HttpGet("nao-lidas")]
    [Authorize]
    public async Task<IActionResult> GetNaoLidas()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var notificacoes = await _notificacaoRepository.GetNaoLidasByUserIdAsync(userId);

        var dtos = notificacoes.Select(n => new NotificacaoDto
        {
            Id = n.Id,
            Titulo = n.Titulo,
            Mensagem = n.Mensagem,
            GrupoId = n.GrupoId,
            GrupoNome = n.Grupo.Name,
            GrupoSigla = n.Grupo.Sigla,
            RemetenteId = n.RemetenteId,
            RemetenteNome = n.Remetente.Name,
            DataEnvio = n.DataEnvio,
            IsAtiva = n.IsAtiva,
            Lida = false
        });

        return Ok(dtos);
    }

    [HttpPost("{id}/marcar-lida")]
    [Authorize]
    public async Task<IActionResult> MarcarComoLida(Guid id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        await _notificacaoRepository.MarcarComoLidaAsync(id, userId);
        return NoContent();
    }

    [HttpGet("grupo/{grupoId}")]
    [Authorize(Roles = "Admin,CoordenadorGeral,CoordenadorGrupo")]
    public async Task<IActionResult> GetByGrupo(Guid grupoId, [FromQuery] bool incluirInativas = false)
    {
        var notificacoes = await _notificacaoRepository.GetByGrupoIdAsync(grupoId, incluirInativas);

        var dtos = notificacoes.Select(n => new NotificacaoDto
        {
            Id = n.Id,
            Titulo = n.Titulo,
            Mensagem = n.Mensagem,
            GrupoId = n.GrupoId,
            GrupoNome = n.Grupo.Name,
            GrupoSigla = n.Grupo.Sigla,
            RemetenteId = n.RemetenteId,
            RemetenteNome = n.Remetente.Name,
            DataEnvio = n.DataEnvio,
            IsAtiva = n.IsAtiva
        });

        return Ok(dtos);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,CoordenadorGeral,CoordenadorGrupo")]
    [ServiceFilter(typeof(AuthorizationFilter))]
    public async Task<IActionResult> Create([FromBody] CreateNotificacaoDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        if (dto.GrupoId.HasValue)
        {
            var grupo = await _grupoRepository.GetByIdAsync(dto.GrupoId.Value);
            if (grupo == null)
                return NotFound("Grupo não encontrado");
        }

        var created = await _notificacaoService.CreateAsync(dto);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var notificacao = await _notificacaoRepository.GetByIdAsync(id);
        if (notificacao == null)
            return NotFound();

        return Ok(new NotificacaoDto
        {
            Id = notificacao.Id,
            Titulo = notificacao.Titulo,
            Mensagem = notificacao.Mensagem,
            GrupoId = notificacao.GrupoId,
            RemetenteId = notificacao.RemetenteId,
            DataEnvio = notificacao.DataEnvio,
            IsAtiva = notificacao.IsAtiva
        });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ServiceFilter(typeof(AuthorizationFilter))]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateNotificacaoDto dto)
    {
        var notificacao = await _notificacaoRepository.GetByIdAsync(id);
        if (notificacao == null)
            return NotFound();

        notificacao.Atualizar(dto.Titulo, dto.Mensagem);
        await _notificacaoRepository.UpdateAsync(notificacao);

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ServiceFilter(typeof(AuthorizationFilter))]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _notificacaoRepository.DeleteAsync(id);
        return NoContent();
    }

    [HttpPatch("{id}/desativar")]
    [Authorize(Roles = "Admin")]
    [ServiceFilter(typeof(AuthorizationFilter))]
    public async Task<IActionResult> Desativar(Guid id)
    {
        var notificacao = await _notificacaoRepository.GetByIdAsync(id);
        if (notificacao == null)
            return NotFound();

        notificacao.Desativar();
        await _notificacaoRepository.UpdateAsync(notificacao);
        return NoContent();
    }

    [HttpPatch("{id}/ativar")]
    [Authorize(Roles = "Admin")]
    [ServiceFilter(typeof(AuthorizationFilter))]
    public async Task<IActionResult> Ativar(Guid id)
    {
        var notificacao = await _notificacaoRepository.GetByIdAsync(id);
        if (notificacao == null)
            return NotFound();

        notificacao.Ativar();
        await _notificacaoRepository.UpdateAsync(notificacao);
        return NoContent();
    }
}
