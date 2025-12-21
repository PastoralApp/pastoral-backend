namespace PA.Domain.Enums;

/// <summary>
/// Define os tipos de postagens
/// </summary>
public enum PostType
{
    /// <summary>
    /// Postagem comum de membro
    /// </summary>
    Comum = 1,

    /// <summary>
    /// Postagem oficial da coordenação
    /// </summary>
    Oficial = 2,

    /// <summary>
    /// Postagem fixada no topo do feed
    /// </summary>
    Fixada = 3,

    /// <summary>
    /// Anúncio importante
    /// </summary>
    Anuncio = 4
}
