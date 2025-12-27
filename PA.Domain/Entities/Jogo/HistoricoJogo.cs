using PA.Domain.Common;

namespace PA.Domain.Entities;


public class HistoricoJogo : Entity
{
    public Guid JogoId { get; set; }
    public virtual Jogo Jogo { get; set; } = null!;
    
    public string Acao { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public DateTime DataHora { get; set; }
    
    public Guid UsuarioId { get; set; }
    public virtual User Usuario { get; set; } = null!;
    
    public string? DadosAntigos { get; set; }
    public string? DadosNovos { get; set; }
}
