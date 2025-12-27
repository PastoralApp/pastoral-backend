namespace PA.Application.DTOs;

public class PartidaDto
{
    public Guid Id { get; set; }
    public Guid ModalidadeId { get; set; }
    public string? ModalidadeNome { get; set; }
    public Guid GrupoAId { get; set; }
    public string? GrupoANome { get; set; }
    public Guid GrupoBId { get; set; }
    public string? GrupoBNome { get; set; }
    public int? PlacarGrupoA { get; set; }
    public int? PlacarGrupoB { get; set; }
    public Guid? VencedorId { get; set; }
    public string? VencedorNome { get; set; }
    public DateTime? DataHora { get; set; }
    public string? Local { get; set; }
    public bool Realizada { get; set; }
    public DateTime? DataRealizacao { get; set; }
    public string? Observacoes { get; set; }
    public bool WalkOver { get; set; }
    public Guid? GrupoWOId { get; set; }
}

public class CreatePartidaDto
{
    public Guid ModalidadeId { get; set; }
    public Guid GrupoAId { get; set; }
    public Guid GrupoBId { get; set; }
    public DateTime? DataHora { get; set; }
    public string? Local { get; set; }
}

public class RegistrarResultadoPartidaDto
{
    public int PlacarGrupoA { get; set; }
    public int PlacarGrupoB { get; set; }
    public bool WalkOver { get; set; } = false;
    public Guid? GrupoWOId { get; set; }
    public string? Observacoes { get; set; }
}
