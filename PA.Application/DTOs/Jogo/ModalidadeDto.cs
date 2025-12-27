using PA.Domain.Enums;

namespace PA.Application.DTOs;

public class ModalidadeDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public TipoModalidade Tipo { get; set; }
    public Guid OlimpiadasId { get; set; }
    public GeneroModalidade CategoriaGenero { get; set; }
    public int PontosVitoria { get; set; }
    public int PontosEmpate { get; set; }
    public int PontosDerrota { get; set; }
    public int PontosOuro { get; set; }
    public int PontosPrata { get; set; }
    public int PontosBronze { get; set; }
    public bool UsaChaveamento { get; set; }
    public FaseJogo? FaseAtual { get; set; }
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public string? LocalRealizacao { get; set; }
    public int? NumeroMaximoParticipantesPorGrupo { get; set; }
    public int? NumeroMinimoParticipantesPorGrupo { get; set; }
    public bool Finalizada { get; set; }
    public List<PartidaDto> Partidas { get; set; } = new();
}

public class CreateModalidadeDto
{
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public TipoModalidade Tipo { get; set; }
    public Guid OlimpiadasId { get; set; }
    public GeneroModalidade CategoriaGenero { get; set; }
    public int PontosVitoria { get; set; } = 3;
    public int PontosEmpate { get; set; } = 1;
    public int PontosDerrota { get; set; } = 0;
    public int PontosOuro { get; set; } = 5;
    public int PontosPrata { get; set; } = 3;
    public int PontosBronze { get; set; } = 1;
    public bool UsaChaveamento { get; set; } = true;
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public string? LocalRealizacao { get; set; }
    public int? NumeroMaximoParticipantesPorGrupo { get; set; }
    public int? NumeroMinimoParticipantesPorGrupo { get; set; }
}

public class UpdateModalidadeDto
{
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public int PontosVitoria { get; set; }
    public int PontosEmpate { get; set; }
    public int PontosDerrota { get; set; }
    public int PontosOuro { get; set; }
    public int PontosPrata { get; set; }
    public int PontosBronze { get; set; }
    public FaseJogo? FaseAtual { get; set; }
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public string? LocalRealizacao { get; set; }
    public bool Finalizada { get; set; }
}
