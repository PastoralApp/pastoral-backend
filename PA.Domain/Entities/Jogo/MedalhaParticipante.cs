using PA.Domain.Common;

namespace PA.Domain.Entities;

public class MedalhaParticipante : Entity
{
    public Guid MedalhaId { get; set; }
    public virtual Medalha Medalha { get; set; } = null!;
    
    public Guid UserId { get; set; }
    public virtual User User { get; set; } = null!;
    
    public DateTime DataRecebimento { get; set; }
}
