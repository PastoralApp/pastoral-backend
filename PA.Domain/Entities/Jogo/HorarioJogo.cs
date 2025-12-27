using PA.Domain.Common;

namespace PA.Domain.Entities;
public class HorarioJogo : Entity
{
    public Guid JogoId { get; set; }
    public virtual Jogo Jogo { get; set; } = null!;
    
    public DateTime DataHora { get; set; }
    public string? Descricao { get; set; }
    public string? Local { get; set; }
    
    public virtual Modalidade? Modalidade { get; set; }
    
    public Guid? ProvaId { get; set; }
    public virtual Prova? Prova { get; set; }
}
