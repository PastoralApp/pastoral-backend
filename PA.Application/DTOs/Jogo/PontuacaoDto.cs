namespace PA.Application.DTOs;

public class PontuacaoProvaDto
{
    public Guid Id { get; set; }
    public Guid ProvaId { get; set; }
    public string? ProvaNome { get; set; }
    public Guid GrupoId { get; set; }
    public string? GrupoNome { get; set; }
    public Guid? ParticipanteId { get; set; }
    public string? ParticipanteNome { get; set; }
    public decimal Pontuacao { get; set; }
    public int? Posicao { get; set; }
    public DateTime DataRegistro { get; set; }
    public Guid RegistradoPorId { get; set; }
    public string? RegistradoPorNome { get; set; }
    public string? Observacoes { get; set; }
}

public class RegistrarPontuacaoProvaDto
{
    public Guid ProvaId { get; set; }
    public Guid GrupoId { get; set; }
    public Guid? ParticipanteId { get; set; }
    public decimal Pontuacao { get; set; }
    public string? Observacoes { get; set; }
}
