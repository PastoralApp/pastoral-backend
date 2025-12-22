using PA.Domain.Common;

namespace PA.Domain.Entities;

public class UserGrupo : Entity
{
    public Guid UserId { get; private set; }
    public Guid GrupoId { get; private set; }
    public DateTime DataEntrada { get; private set; }
    public bool IsAtivo { get; private set; }
    public bool SilenciarNotificacoes { get; private set; }
    public User User { get; private set; } = null!;
    public Grupo Grupo { get; private set; } = null!;

    private UserGrupo() { }

    public UserGrupo(Guid userId, Guid grupoId)
    {
        UserId = userId;
        GrupoId = grupoId;
        DataEntrada = DateTime.UtcNow;
        IsAtivo = true;
        SilenciarNotificacoes = false;
    }

    public void Desativar() => IsAtivo = false;
    public void Ativar() => IsAtivo = true;
    public void SilenciarNotificacoesDoGrupo() => SilenciarNotificacoes = true;
    public void AtivarNotificacoesDoGrupo() => SilenciarNotificacoes = false;
}
