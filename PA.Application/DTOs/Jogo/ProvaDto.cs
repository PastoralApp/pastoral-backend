using PA.Domain.Enums;

namespace PA.Application.DTOs;

public class ProvaDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public TipoProva TipoProva { get; set; }
    public Guid GuiaId { get; set; }
    public decimal PontuacaoMaxima { get; set; }
    public decimal PontuacaoMinima { get; set; }
    public bool EhIndividual { get; set; }
    public bool EhPorGrupo { get; set; }
    public DateTime? DataRealizacao { get; set; }
    public string? LocalRealizacao { get; set; }
    public string? CriteriosAvaliacao { get; set; }
    public int Ordem { get; set; }
    public bool Realizada { get; set; }
    public DateTime? DataRealizacaoEfetiva { get; set; }
    public Guid? CoordenadorResponsavelId { get; set; }
    public string? CoordenadorResponsavelNome { get; set; }
    public List<PontuacaoProvaDto> Pontuacoes { get; set; } = new();
}

public class CreateProvaDto
{
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public TipoProva TipoProva { get; set; }
    public Guid GuiaId { get; set; }
    public decimal PontuacaoMaxima { get; set; }
    public decimal PontuacaoMinima { get; set; } = 0;
    public bool EhIndividual { get; set; } = false;
    public bool EhPorGrupo { get; set; } = true;
    public DateTime? DataRealizacao { get; set; }
    public string? LocalRealizacao { get; set; }
    public string? CriteriosAvaliacao { get; set; }
    public int Ordem { get; set; }
    public Guid? CoordenadorResponsavelId { get; set; }
}

public class UpdateProvaDto
{
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public decimal PontuacaoMaxima { get; set; }
    public decimal PontuacaoMinima { get; set; }
    public DateTime? DataRealizacao { get; set; }
    public string? LocalRealizacao { get; set; }
    public string? CriteriosAvaliacao { get; set; }
    public int Ordem { get; set; }
    public bool Realizada { get; set; }
    public DateTime? DataRealizacaoEfetiva { get; set; }
}
