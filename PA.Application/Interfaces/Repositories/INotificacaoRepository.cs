using PA.Domain.Entities;
using PA.Domain.Interfaces;

namespace PA.Application.Interfaces.Repositories;

public interface INotificacaoRepository : IRepository<Notificacao>
{
    Task<IEnumerable<Notificacao>> GetByGrupoIdAsync(Guid grupoId, bool incluirInativas = false);
    Task<IEnumerable<Notificacao>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<Notificacao>> GetNaoLidasByUserIdAsync(Guid userId);
    Task<NotificacaoLeitura?> GetLeituraAsync(Guid notificacaoId, Guid userId);
    Task MarcarComoLidaAsync(Guid notificacaoId, Guid userId);
}
