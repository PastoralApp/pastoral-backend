using PA.Domain.Common;

namespace PA.Domain.Entities;

public class PontuacaoProva : Entity
{
    public Guid ProvaId { get; set; }
    public virtual Prova Prova { get; set; } = null!;
    
    public Guid GrupoId { get; set; }
    public virtual Grupo Grupo { get; set; } = null!;
    
    public Guid? ParticipanteId { get; set; }
    public virtual User? Participante { get; set; }
    
    public decimal Pontuacao { get; set; }
    public int? Posicao { get; set; }
    
    public DateTime DataRegistro { get; set; }
    public Guid RegistradoPorId { get; set; }
    public virtual User RegistradoPor { get; set; } = null!;
    
    public string? Observacoes { get; set; }
}
