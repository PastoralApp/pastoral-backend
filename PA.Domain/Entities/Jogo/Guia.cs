using PA.Domain.Enums;

namespace PA.Domain.Entities;
public class Guia : Jogo
{
    public Guia()
    {
        TipoDeJogo = TipoDeJogo.Guia;
    }
    
    public virtual ICollection<Prova> Provas { get; set; } = new List<Prova>();
    
    public virtual ICollection<PontuacaoProva> PontuacoesProvas { get; set; } = new List<PontuacaoProva>();
    
    public virtual ICollection<RankingGuia> Rankings { get; set; } = new List<RankingGuia>();
    
    public string? Tema { get; set; }
    
    public bool PermiteProvasIndividuais { get; set; } = false;
    public bool PermiteProvasGrupo { get; set; } = true;
    public int? PontuacaoMinimaPorProva { get; set; }
    public int? PontuacaoMaximaPorProva { get; set; }
}
