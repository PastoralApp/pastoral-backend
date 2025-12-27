using PA.Domain.Common;

namespace PA.Domain.Entities;
public class GrupoModalidade : Entity
{
    public Guid GrupoId { get; set; }
    public virtual Grupo Grupo { get; set; } = null!;
    
    public Guid ModalidadeId { get; set; }
    public virtual Modalidade Modalidade { get; set; } = null!;
    
    public DateTime DataInscricao { get; set; }
    public bool Confirmado { get; set; } = false;
    
    public virtual ICollection<ParticipanteModalidade> Participantes { get; set; } = new List<ParticipanteModalidade>();
}
