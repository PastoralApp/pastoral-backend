using PA.Domain.Common;

namespace PA.Domain.Entities;

public class Trofeu : Entity
{
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    
    public Guid JogoId { get; set; }
    public virtual Jogo Jogo { get; set; } = null!;
    
    public Guid GrupoId { get; set; }
    public virtual Grupo Grupo { get; set; } = null!;
    
    public int Ano { get; set; }
    public DateTime DataConquista { get; set; }
    
    public string? Categoria { get; set; }
    
    public int? Posicao { get; set; }
    
    public string? ImagemUrl { get; set; }
    
    public string? Observacoes { get; set; }
}
