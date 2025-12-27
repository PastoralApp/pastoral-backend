namespace PA.Application.DTOs;

public class RankingOlimpiadasDto
{
    public Guid Id { get; set; }
    public Guid OlimpiadasId { get; set; }
    public Guid GrupoId { get; set; }
    public string? GrupoNome { get; set; }
    public string? GrupoCor { get; set; }
    public string? GrupoImagemUrl { get; set; }
    public int Posicao { get; set; }
    public decimal PontuacaoTotal { get; set; }
    public int MedalhasOuro { get; set; }
    public int MedalhasPrata { get; set; }
    public int MedalhasBronze { get; set; }
    public DateTime DataAtualizacao { get; set; }
}

public class RankingGuiaDto
{
    public Guid Id { get; set; }
    public Guid GuiaId { get; set; }
    public Guid GrupoId { get; set; }
    public string? GrupoNome { get; set; }
    public string? GrupoCor { get; set; }
    public string? GrupoImagemUrl { get; set; }
    public int Posicao { get; set; }
    public decimal PontuacaoTotal { get; set; }
    public int ProvasRealizadas { get; set; }
    public decimal MediaPontuacao { get; set; }
    public DateTime DataAtualizacao { get; set; }
}
