using PA.Domain.Common;

namespace PA.Domain.Entities;

/// <summary>
/// Relacionamento Many-to-Many entre User e Grupo
/// </summary>
public class UserGrupo : Entity
{
    public Guid UserId { get; private set; }
    public Guid GrupoId { get; private set; }
    public DateTime DataEntrada { get; private set; }
    public bool IsAtivo { get; private set; }

    // Navigation
    public User User { get; private set; } = null!;
    public Grupo Grupo { get; private set; } = null!;

    private UserGrupo() { }

    public UserGrupo(Guid userId, Guid grupoId)
    {
        UserId = userId;
        GrupoId = grupoId;
        DataEntrada = DateTime.UtcNow;
        IsAtivo = true;
    }

    public void Desativar() => IsAtivo = false;
    public void Ativar() => IsAtivo = true;
}
