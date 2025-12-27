using PA.Domain.Common;

namespace PA.Domain.Entities;

public class PontuacaoJogo : Entity
{
    public Guid JogoId { get; set; }
    public virtual Jogo Jogo { get; set; } = null!;
    
    public Guid GrupoId { get; set; }
    public virtual Grupo Grupo { get; set; } = null!;
    
    public decimal Pontos { get; set; }
    public int? Posicao { get; set; }
    
    public decimal? PontosOuro { get; set; }
    public decimal? PontosPrata { get; set; }
    public decimal? PontosBronze { get; set; }
    public decimal? PontosAdicionais { get; set; }
    
    public DateTime DataAtualizacao { get; set; }
}
