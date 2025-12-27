using PA.Application.DTOs;

namespace PA.Application.Interfaces.Services;

public interface INotificacaoHubService
{
    Task NotificarUsuarioAsync(Guid usuarioId, NotificacaoDto notificacao);
    Task NotificarGrupoAsync(Guid grupoId, NotificacaoDto notificacao);
    Task NotificarTodosAsync(NotificacaoDto notificacao);
    Task NotificarMultiplosUsuariosAsync(IEnumerable<Guid> usuarioIds, NotificacaoDto notificacao);
}
