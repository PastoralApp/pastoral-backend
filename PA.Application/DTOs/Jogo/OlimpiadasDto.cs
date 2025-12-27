using PA.Domain.Enums;

namespace PA.Application.DTOs;

public class OlimpiadasDto : JogoDto
{
    public int QuantidadeFinaisSemana { get; set; }
    public List<DateTime> FinaisSemana { get; set; } = new();
    public int PontosOuro { get; set; }
    public int PontosPrata { get; set; }
    public int PontosBronze { get; set; }
    public int PontosParticipacao { get; set; }
    public bool UsaFaseGrupos { get; set; }
    public bool UsaMataMata { get; set; }
    public int? QuantidadeGruposPorFase { get; set; }
    public int? QuantidadeClassificadosPorGrupo { get; set; }
    public List<ModalidadeDto> Modalidades { get; set; } = new();
    public List<RankingOlimpiadasDto> Rankings { get; set; } = new();
}

public class CreateOlimpiadasDto : CreateJogoDto
{
    public int QuantidadeFinaisSemana { get; set; } = 1;
    public List<DateTime> FinaisSemana { get; set; } = new();
    public int PontosOuro { get; set; } = 5;
    public int PontosPrata { get; set; } = 3;
    public int PontosBronze { get; set; } = 1;
    public int PontosParticipacao { get; set; } = 0;
    public bool UsaFaseGrupos { get; set; } = false;
    public bool UsaMataMata { get; set; } = true;
    public int? QuantidadeGruposPorFase { get; set; }
    public int? QuantidadeClassificadosPorGrupo { get; set; }
}

public class UpdateOlimpiadasDto : UpdateJogoDto
{
    public int QuantidadeFinaisSemana { get; set; }
    public List<DateTime> FinaisSemana { get; set; } = new();
    public int PontosOuro { get; set; }
    public int PontosPrata { get; set; }
    public int PontosBronze { get; set; }
    public int PontosParticipacao { get; set; }
    public bool UsaFaseGrupos { get; set; }
    public bool UsaMataMata { get; set; }
    public int? QuantidadeGruposPorFase { get; set; }
    public int? QuantidadeClassificadosPorGrupo { get; set; }
}
