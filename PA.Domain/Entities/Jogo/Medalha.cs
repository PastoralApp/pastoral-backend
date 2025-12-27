using PA.Domain.Common;
using PA.Domain.Enums;

namespace PA.Domain.Entities;

public class Medalha : Entity
{
    public TipoMedalha Tipo { get; set; }
    public Guid JogoId { get; set; }
    public virtual Jogo Jogo { get; set; } = null!;
    
    public Guid? ModalidadeId { get; set; }
    public virtual Modalidade? Modalidade { get; set; }
    
    public Guid? ProvaId { get; set; }
    public virtual Prova? Prova { get; set; }
    
    public Guid GrupoId { get; set; }
    public virtual Grupo Grupo { get; set; } = null!;
    
    public virtual ICollection<MedalhaParticipante> Participantes { get; set; } = new List<MedalhaParticipante>();
    
    public int Ano { get; set; }
    public DateTime DataConquista { get; set; }
    
    public string? Descricao { get; set; }
    public string? Observacoes { get; set; }
}
