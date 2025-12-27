using PA.Domain.Common;

namespace PA.Domain.Entities;

public class RankingOlimpiadas : Entity
{
    public Guid OlimpiadasId { get; set; }
    public virtual Olimpiadas Olimpiadas { get; set; } = null!;
    
    public Guid GrupoId { get; set; }
    public virtual Grupo Grupo { get; set; } = null!;
    
    public int Posicao { get; set; }
    public decimal PontuacaoTotal { get; set; }
    
    public int MedalhasOuro { get; set; }
    public int MedalhasPrata { get; set; }
    public int MedalhasBronze { get; set; }
    
    public DateTime DataAtualizacao { get; set; }
}
