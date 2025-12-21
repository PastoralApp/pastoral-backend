namespace PA.Domain.Enums;

/// <summary>
/// Define os tipos de permissões no sistema
/// </summary>
public enum RoleType
{
    /// <summary>
    /// Usuário comum (membro)
    /// </summary>
    Usuario = 1,

    /// <summary>
    /// Coordenador de grupo específico
    /// </summary>
    CoordenadorGrupo = 2,

    /// <summary>
    /// Coordenador geral da pastoral
    /// </summary>
    CoordenadorGeral = 3,

    /// <summary>
    /// Administrador do sistema
    /// </summary>
    Admin = 4
}
