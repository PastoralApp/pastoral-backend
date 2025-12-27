using Microsoft.AspNetCore.SignalR;
using PA.Application.DTOs;
using PA.Application.Interfaces.Services;

namespace PA.API.Services;

public class NotificacaoHubService : INotificacaoHubService
{
    private readonly IHubContext<Hubs.NotificacaoHub> _hubContext;
    private readonly ILogger<NotificacaoHubService> _logger;

    public NotificacaoHubService(
        IHubContext<Hubs.NotificacaoHub> hubContext,
        ILogger<NotificacaoHubService> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task NotificarUsuarioAsync(Guid usuarioId, NotificacaoDto notificacao)
    {
        try
        {
            var groupName = $"user_{usuarioId}";
            _logger.LogInformation($"Enviando notificação para usuário {usuarioId}");
            
            await _hubContext.Clients
                .Group(groupName)
                .SendAsync("NovaNotificacao", notificacao);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao notificar usuário {usuarioId}: {ex.Message}");
        }
    }

    public async Task NotificarGrupoAsync(Guid grupoId, NotificacaoDto notificacao)
    {
        try
        {
            var groupName = $"grupo_{grupoId}";
            _logger.LogInformation($"Enviando notificação para grupo {grupoId}");
            
            await _hubContext.Clients
                .Group(groupName)
                .SendAsync("NovaNotificacao", notificacao);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao notificar grupo {grupoId}: {ex.Message}");
        }
    }

    public async Task NotificarTodosAsync(NotificacaoDto notificacao)
    {
        try
        {
            _logger.LogInformation("Enviando notificação para todos os usuários");
            
            await _hubContext.Clients
                .All
                .SendAsync("NovaNotificacao", notificacao);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao notificar todos: {ex.Message}");
        }
    }

    public async Task NotificarMultiplosUsuariosAsync(IEnumerable<Guid> usuarioIds, NotificacaoDto notificacao)
    {
        try
        {
            foreach (var usuarioId in usuarioIds)
            {
                await NotificarUsuarioAsync(usuarioId, notificacao);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao notificar múltiplos usuários: {ex.Message}");
        }
    }
}
