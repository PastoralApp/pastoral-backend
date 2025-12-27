using PA.Domain.Enums;

namespace PA.Application.DTOs;

public class JogoDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public TipoDeJogo TipoDeJogo { get; set; }
    public StatusJogo Status { get; set; }
    public DateTime DataInicio { get; set; }
    public DateTime DataFim { get; set; }
    public int Ano { get; set; }
    public List<string> Tags { get; set; } = new();
    public Guid PastoralId { get; set; }
    public string? PastoralNome { get; set; }
    public Guid CriadoPorId { get; set; }
    public string? CriadoPorNome { get; set; }
    public GeneroModalidade Genero { get; set; }
    public SistemaChave? SistemaChave { get; set; }
    public string? ImagemCapaUrl { get; set; }
    public string? RegulamentoUrl { get; set; }
    public bool PermiteInscricao { get; set; }
    public DateTime? DataLimiteInscricao { get; set; }
    public string? ObservacoesGerais { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateJogoDto
{
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public TipoDeJogo TipoDeJogo { get; set; }
    public DateTime DataInicio { get; set; }
    public DateTime DataFim { get; set; }
    public int Ano { get; set; }
    public List<string> Tags { get; set; } = new();
    public Guid PastoralId { get; set; }
    public GeneroModalidade Genero { get; set; }
    public SistemaChave? SistemaChave { get; set; }
    public string? ImagemCapaUrl { get; set; }
    public string? RegulamentoUrl { get; set; }
    public DateTime? DataLimiteInscricao { get; set; }
    public string? ObservacoesGerais { get; set; }
}

public class UpdateJogoDto
{
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public StatusJogo Status { get; set; }
    public DateTime DataInicio { get; set; }
    public DateTime DataFim { get; set; }
    public List<string> Tags { get; set; } = new();
    public GeneroModalidade Genero { get; set; }
    public string? ImagemCapaUrl { get; set; }
    public string? RegulamentoUrl { get; set; }
    public bool PermiteInscricao { get; set; }
    public DateTime? DataLimiteInscricao { get; set; }
    public string? ObservacoesGerais { get; set; }
}
