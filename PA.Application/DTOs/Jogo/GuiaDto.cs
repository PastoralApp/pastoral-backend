namespace PA.Application.DTOs;

public class GuiaDto : JogoDto
{
    public string? Tema { get; set; }
    public bool PermiteProvasIndividuais { get; set; }
    public bool PermiteProvasGrupo { get; set; }
    public int? PontuacaoMinimaPorProva { get; set; }
    public int? PontuacaoMaximaPorProva { get; set; }
    public List<ProvaDto> Provas { get; set; } = new();
    public List<RankingGuiaDto> Rankings { get; set; } = new();
}

public class CreateGuiaDto : CreateJogoDto
{
    public string? Tema { get; set; }
    public bool PermiteProvasIndividuais { get; set; } = false;
    public bool PermiteProvasGrupo { get; set; } = true;
    public int? PontuacaoMinimaPorProva { get; set; }
    public int? PontuacaoMaximaPorProva { get; set; }
}

public class UpdateGuiaDto : UpdateJogoDto
{
    public string? Tema { get; set; }
    public bool PermiteProvasIndividuais { get; set; }
    public bool PermiteProvasGrupo { get; set; }
    public int? PontuacaoMinimaPorProva { get; set; }
    public int? PontuacaoMaximaPorProva { get; set; }
}
