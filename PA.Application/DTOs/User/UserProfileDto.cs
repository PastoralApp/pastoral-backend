using PA.Domain.Enums;

namespace PA.Application.DTOs;

public class UserProfileDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    public string? ImagemPerfilUrl { get; set; }
    public DateTime DataCadastro { get; set; }
    public List<UserGrupoSimplifiedDto> Grupos { get; set; } = new();
    public List<MedalhaSimplifiedDto> Medalhas { get; set; } = new();
    public List<TrofeuSimplifiedDto> Trofeus { get; set; } = new();
    public UserStatsDto Estatisticas { get; set; } = new();
}

public class UserGrupoSimplifiedDto
{
    public Guid GrupoId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Sigla { get; set; }
    public string? LogoUrl { get; set; }
    public string? PrimaryColor { get; set; }
    public string? SecondaryColor { get; set; }
    public DateTime DataEntrada { get; set; }
    public bool IsAtivo { get; set; }
    public string? FuncaoNoGrupo { get; set; }
}

public class MedalhaSimplifiedDto
{
    public Guid Id { get; set; }
    public TipoMedalha Tipo { get; set; }
    public string? JogoNome { get; set; }
    public string? ModalidadeNome { get; set; }
    public string? GrupoNome { get; set; }
    public int Ano { get; set; }
    public DateTime DataConquista { get; set; }
}

public class TrofeuSimplifiedDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? JogoNome { get; set; }
    public string? GrupoNome { get; set; }
    public int Ano { get; set; }
    public DateTime DataConquista { get; set; }
    public int? Posicao { get; set; }
    public string? ImagemUrl { get; set; }
}

public class UserStatsDto
{
    public int TotalMedalhas { get; set; }
    public int MedalhasOuro { get; set; }
    public int MedalhasPrata { get; set; }
    public int MedalhasBronze { get; set; }
    public int TotalTrofeus { get; set; }
    public int TotalGrupos { get; set; }
    public int GruposAtivos { get; set; }
    public int JogosParticipados { get; set; }
}

public class UserSelectDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? ImagemPerfilUrl { get; set; }
    public string? NomeGrupo { get; set; }
    public bool IsAtivo { get; set; }
}

public class UserSearchDto
{
    public string? Nome { get; set; }
    public string? Email { get; set; }
    public Guid? GrupoId { get; set; }
    public Guid? PastoralId { get; set; }
    public bool? ApenasAtivos { get; set; }
    public int? Page { get; set; }
    public int? PageSize { get; set; }
}

public class UserSearchResultDto
{
    public List<UserSelectDto> Users { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}
