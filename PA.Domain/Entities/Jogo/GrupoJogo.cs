using PA.Domain.Common;

namespace PA.Domain.Entities;

public class GrupoJogo : Entity
{
    public Guid GrupoId { get; set; }
    public virtual Grupo Grupo { get; set; } = null!;
    
    public Guid JogoId { get; set; }
    public virtual Jogo Jogo { get; set; } = null!;
    
    public DateTime DataInscricao { get; set; }
    public bool Confirmado { get; set; } = false;
    public string? Observacoes { get; set; }
}
