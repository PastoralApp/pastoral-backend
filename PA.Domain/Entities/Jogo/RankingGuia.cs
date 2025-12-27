using PA.Domain.Common;

namespace PA.Domain.Entities;

public class RankingGuia : Entity
{
    public Guid GuiaId { get; set; }
    public virtual Guia Guia { get; set; } = null!;
    
    public Guid GrupoId { get; set; }
    public virtual Grupo Grupo { get; set; } = null!;
    
    public int Posicao { get; set; }
    public decimal PontuacaoTotal { get; set; }
    
    public int ProvasRealizadas { get; set; }
    public decimal MediaPontuacao { get; set; }
    
    public DateTime DataAtualizacao { get; set; }
}
