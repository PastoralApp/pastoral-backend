namespace PA.Application.DTOs;

public class NotificacaoDto
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Mensagem { get; set; } = string.Empty;
    public Guid? GrupoId { get; set; }
    public bool IsGeral { get; set; }
    public string? GrupoNome { get; set; }
    public string? GrupoSigla { get; set; }
    public Guid RemetenteId { get; set; }
    public string? RemetenteNome { get; set; }
    public DateTime DataEnvio { get; set; }
    public bool IsAtiva { get; set; }
    public bool Lida { get; set; }
    public DateTime? DataLeitura { get; set; }
}

public record CreateNotificacaoDto(
    string Titulo,
    string Mensagem,
    Guid? GrupoId,
    bool IsGeral
);

public record UpdateNotificacaoDto(
    string Titulo,
    string Mensagem
);
