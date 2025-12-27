using PA.Domain.Common;
using PA.Domain.Enums;

namespace PA.Domain.Entities;

public class Modalidade : Entity
{
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public TipoModalidade Tipo { get; set; }
    
    public Guid OlimpiadasId { get; set; }
    public virtual Olimpiadas Olimpiadas { get; set; } = null!;
    
    public GeneroModalidade CategoriaGenero { get; set; }
    
    public int PontosVitoria { get; set; } = 3;
    public int PontosEmpate { get; set; } = 1;
    public int PontosDerrota { get; set; } = 0;
    public int PontosOuro { get; set; } = 5;
    public int PontosPrata { get; set; } = 3;
    public int PontosBronze { get; set; } = 1;
    
    public virtual ICollection<GrupoModalidade> GruposParticipantes { get; set; } = new List<GrupoModalidade>();
    
    public virtual ICollection<Partida> Partidas { get; set; } = new List<Partida>();
    
    public virtual ICollection<ChaveModalidade> Chaves { get; set; } = new List<ChaveModalidade>();
    
    public bool UsaChaveamento { get; set; } = true;
    public FaseJogo? FaseAtual { get; set; }
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public string? LocalRealizacao { get; set; }
    public int? NumeroMaximoParticipantesPorGrupo { get; set; }
    public int? NumeroMinimoParticipantesPorGrupo { get; set; }
    
    public bool Finalizada { get; set; } = false;
}
