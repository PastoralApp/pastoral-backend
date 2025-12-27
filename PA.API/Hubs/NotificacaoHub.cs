using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace PA.API.Hubs;

[Authorize]
public class NotificacaoHub : Hub
{
    private readonly ILogger<NotificacaoHub> _logger;

    public NotificacaoHub(ILogger<NotificacaoHub> logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            Context.Abort();
            return;
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
        
        _logger.LogInformation($"Usuário {userId} conectado ao Hub de Notificações. ConnectionId: {Context.ConnectionId}");
        
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        _logger.LogInformation($"Usuário {userId} desconectado do Hub. ConnectionId: {Context.ConnectionId}");
        
        await base.OnDisconnectedAsync(exception);
    }

    public async Task TestConnection()
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        await Clients.Caller.SendAsync("TestResponse", $"Conectado com sucesso! UserID: {userId}");
    }
}
