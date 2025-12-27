using PA.Domain.Common;

namespace PA.Domain.Entities;

public class GrupoChave : Entity
{
    public Guid GrupoId { get; set; }
    public virtual Grupo Grupo { get; set; } = null!;
    
    public Guid? ChaveOlimpiadasId { get; set; }
    public virtual ChaveOlimpiadas? ChaveOlimpiadas { get; set; }
    
    public Guid? ChaveModalidadeId { get; set; }
    public virtual ChaveModalidade? ChaveModalidade { get; set; }
    
    public int Posicao { get; set; }
    public bool Classificado { get; set; } = false;
}
