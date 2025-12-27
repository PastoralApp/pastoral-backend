using PA.Domain.Common;

namespace PA.Domain.Entities;

public class Partida : Entity
{
    public Guid ModalidadeId { get; set; }
    public virtual Modalidade Modalidade { get; set; } = null!;
    
    public Guid? ChaveModalidadeId { get; set; }
    public virtual ChaveModalidade? ChaveModalidade { get; set; }
    
    public Guid GrupoAId { get; set; }
    public virtual Grupo GrupoA { get; set; } = null!;
    
    public Guid GrupoBId { get; set; }
    public virtual Grupo GrupoB { get; set; } = null!;
    
    public int? PlacarGrupoA { get; set; }
    public int? PlacarGrupoB { get; set; }
    
    public Guid? VencedorId { get; set; }
    public virtual Grupo? Vencedor { get; set; }
    
    public DateTime? DataHora { get; set; }
    public string? Local { get; set; }
    
    public bool Realizada { get; set; } = false;
    public DateTime? DataRealizacao { get; set; }
    
    public string? Observacoes { get; set; }
    
    public bool WalkOver { get; set; } = false;
    public Guid? GrupoWOId { get; set; }
}
