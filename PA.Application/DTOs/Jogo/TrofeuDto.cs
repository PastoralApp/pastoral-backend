namespace PA.Application.DTOs;

public class TrofeuDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public Guid JogoId { get; set; }
    public string? JogoNome { get; set; }
    public Guid GrupoId { get; set; }
    public string? GrupoNome { get; set; }
    public int Ano { get; set; }
    public DateTime DataConquista { get; set; }
    public string? Categoria { get; set; }
    public int? Posicao { get; set; }
    public string? ImagemUrl { get; set; }
    public string? Observacoes { get; set; }
}

public class CreateTrofeuDto
{
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public Guid JogoId { get; set; }
    public Guid GrupoId { get; set; }
    public int Ano { get; set; }
    public DateTime DataConquista { get; set; }
    public string? Categoria { get; set; }
    public int? Posicao { get; set; }
    public string? ImagemUrl { get; set; }
    public string? Observacoes { get; set; }
}
