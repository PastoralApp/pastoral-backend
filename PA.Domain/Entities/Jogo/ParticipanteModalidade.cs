using PA.Domain.Common;

namespace PA.Domain.Entities;

public class ParticipanteModalidade : Entity
{
    public Guid UserId { get; set; }
    public virtual User User { get; set; } = null!;
    
    public Guid GrupoModalidadeId { get; set; }
    public virtual GrupoModalidade GrupoModalidade { get; set; } = null!;
    
    public DateTime DataInscricao { get; set; }
    public bool Confirmado { get; set; } = true;

    public string? Posicao { get; set; }
}
