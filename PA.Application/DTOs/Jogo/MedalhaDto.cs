using PA.Domain.Enums;

namespace PA.Application.DTOs;

public class MedalhaDto
{
    public Guid Id { get; set; }
    public TipoMedalha Tipo { get; set; }
    public Guid JogoId { get; set; }
    public string? JogoNome { get; set; }
    public Guid? ModalidadeId { get; set; }
    public string? ModalidadeNome { get; set; }
    public Guid? ProvaId { get; set; }
    public string? ProvaNome { get; set; }
    public Guid GrupoId { get; set; }
    public string? GrupoNome { get; set; }
    public int Ano { get; set; }
    public DateTime DataConquista { get; set; }
    public string? Descricao { get; set; }
    public string? Observacoes { get; set; }
    public List<UserDto> Participantes { get; set; } = new();
}

public class CreateMedalhaDto
{
    public TipoMedalha Tipo { get; set; }
    public Guid JogoId { get; set; }
    public Guid? ModalidadeId { get; set; }
    public Guid? ProvaId { get; set; }
    public Guid GrupoId { get; set; }
    public int Ano { get; set; }
    public DateTime DataConquista { get; set; }
    public string? Descricao { get; set; }
    public string? Observacoes { get; set; }
    public List<Guid> ParticipantesIds { get; set; } = new();
}
