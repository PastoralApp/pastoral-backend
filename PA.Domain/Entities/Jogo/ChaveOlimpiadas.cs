using PA.Domain.Common;
using PA.Domain.Enums;

namespace PA.Domain.Entities;

public class ChaveOlimpiadas : Entity
{
    public Guid OlimpiadasId { get; set; }
    public virtual Olimpiadas Olimpiadas { get; set; } = null!;
    
    public FaseJogo Fase { get; set; }
    public string Nome { get; set; } = string.Empty;
    
    public virtual ICollection<GrupoChave> Grupos { get; set; } = new List<GrupoChave>();
    
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public bool Finalizada { get; set; } = false;
}
