using PA.Domain.Common;

namespace PA.Domain.Entities;

public class ParticipanteGrupo : Entity
{
    public Guid UserId { get; set; }
    public virtual User User { get; set; } = null!;
    
    public Guid GrupoId { get; set; }
    public virtual Grupo Grupo { get; set; } = null!;
    
    public DateTime DataEntrada { get; set; }
    public DateTime? DataSaida { get; set; }
    public bool Ativo { get; set; } = true;
    
    public string? FuncaoNoGrupo { get; set; }
}
