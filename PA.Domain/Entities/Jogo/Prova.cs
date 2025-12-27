using PA.Domain.Common;
using PA.Domain.Enums;

namespace PA.Domain.Entities;
public class Prova : Entity
{
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public TipoProva TipoProva { get; set; }
    
    public Guid GuiaId { get; set; }
    public virtual Guia Guia { get; set; } = null!;
    
    public decimal PontuacaoMaxima { get; set; }
    public decimal PontuacaoMinima { get; set; } = 0;
    
    public bool EhIndividual { get; set; } = false;
    public bool EhPorGrupo { get; set; } = true;
    
    public virtual ICollection<PontuacaoProva> Pontuacoes { get; set; } = new List<PontuacaoProva>();
    
    public DateTime? DataRealizacao { get; set; }
    public string? LocalRealizacao { get; set; }
    
    public string? CriteriosAvaliacao { get; set; }
    
    public int Ordem { get; set; }
    
    public bool Realizada { get; set; } = false;
    public DateTime? DataRealizacaoEfetiva { get; set; }
    
    public Guid? CoordenadorResponsavelId { get; set; }
    public virtual User? CoordenadorResponsavel { get; set; }
}
