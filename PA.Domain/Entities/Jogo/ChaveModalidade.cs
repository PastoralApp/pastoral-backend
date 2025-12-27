using PA.Domain.Common;
using PA.Domain.Enums;

namespace PA.Domain.Entities;

public class ChaveModalidade : Entity
{
    public Guid ModalidadeId { get; set; }
    public virtual Modalidade Modalidade { get; set; } = null!;
    
    public FaseJogo Fase { get; set; }
    public string Nome { get; set; } = string.Empty;
    
    public virtual ICollection<GrupoChave> Grupos { get; set; } = new List<GrupoChave>();
    
    public virtual ICollection<Partida> Partidas { get; set; } = new List<Partida>();
    
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public bool Finalizada { get; set; } = false;
}
