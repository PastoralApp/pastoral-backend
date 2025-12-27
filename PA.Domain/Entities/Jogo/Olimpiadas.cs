using PA.Domain.Enums;

namespace PA.Domain.Entities;


public class Olimpiadas : Jogo
{
    public Olimpiadas()
    {
        TipoDeJogo = TipoDeJogo.Olimpiadas;
    }
    
    public virtual ICollection<Modalidade> Modalidades { get; set; } = new List<Modalidade>();
    
    public int QuantidadeFinaisSemana { get; set; } = 1;
    public List<DateTime> FinaisSemana { get; set; } = new();
    
    public int PontosOuro { get; set; } = 1;
    public int PontosPrata { get; set; } = 2;
    public int PontosBronze { get; set; } = 3;
    public int PontosParticipacao { get; set; } = 0;
    
    public virtual ICollection<ChaveOlimpiadas> Chaves { get; set; } = new List<ChaveOlimpiadas>();
    
    public bool UsaFaseGrupos { get; set; } = false;
    public bool UsaMataMata { get; set; } = true;
    public int? QuantidadeGruposPorFase { get; set; }
    public int? QuantidadeClassificadosPorGrupo { get; set; }
    
    public virtual ICollection<RankingOlimpiadas> Rankings { get; set; } = new List<RankingOlimpiadas>();
}
