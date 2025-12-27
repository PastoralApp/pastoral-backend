using PA.Domain.Common;
using PA.Domain.Enums;

namespace PA.Domain.Entities;

public abstract class Jogo : Entity
{
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public TipoDeJogo TipoDeJogo { get; protected set; }
    public StatusJogo Status { get; set; }
    public DateTime DataInicio { get; set; }
    public DateTime DataFim { get; set; }
    public int Ano { get; set; }
    
    public List<string> Tags { get; set; } = new();
    
    public Guid PastoralId { get; set; }
    public virtual Pastoral Pastoral { get; set; } = null!;
    
    public Guid CriadoPorId { get; set; }
    public virtual User CriadoPor { get; set; } = null!;
    
    public virtual ICollection<GrupoJogo> GruposParticipantes { get; set; } = new List<GrupoJogo>();
    
    public GeneroModalidade Genero { get; set; }
    
    public SistemaChave? SistemaChave { get; set; }
    
    public virtual ICollection<Medalha> Medalhas { get; set; } = new List<Medalha>();
    public virtual ICollection<Trofeu> Trofeus { get; set; } = new List<Trofeu>();
    
    public virtual ICollection<PontuacaoJogo> Pontuacoes { get; set; } = new List<PontuacaoJogo>();
    
    public virtual ICollection<HistoricoJogo> Historico { get; set; } = new List<HistoricoJogo>();
    
    public virtual ICollection<HorarioJogo> Horarios { get; set; } = new List<HorarioJogo>();
    
    public string? ImagemCapaUrl { get; set; }
    public string? RegulamentoUrl { get; set; }
    
    public bool PermiteInscricao { get; set; } = true;
    public DateTime? DataLimiteInscricao { get; set; }
    public string? ObservacoesGerais { get; set; }
}
