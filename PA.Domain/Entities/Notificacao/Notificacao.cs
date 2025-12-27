using PA.Domain.Common;

namespace PA.Domain.Entities;

public class Notificacao : AggregateRoot
{
    public string Titulo { get; private set; }
    public string Mensagem { get; private set; }
    public Guid? GrupoId { get; private set; }
    public Guid? DestinatarioId { get; private set; }
    public Guid RemetenteId { get; private set; }
    public DateTime DataEnvio { get; private set; }
    public bool IsAtiva { get; private set; }
    public bool IsGeral { get; private set; }
    public bool SendEmail { get; private set; }
    public Grupo? Grupo { get; private set; }
    public User? Destinatario { get; private set; }
    public User Remetente { get; private set; } = null!;
    public ICollection<NotificacaoLeitura> Leituras { get; private set; }

    private Notificacao()
    {
        Titulo = string.Empty;
        Mensagem = string.Empty;
        Leituras = new List<NotificacaoLeitura>();
    }

    public Notificacao(string titulo, string mensagem, Guid remetenteId, Guid? grupoId = null, Guid? destinatarioId = null, bool isGeral = false, bool sendEmail = false)
    {
        Titulo = titulo;
        Mensagem = mensagem;
        GrupoId = grupoId;
        DestinatarioId = destinatarioId;
        RemetenteId = remetenteId;
        DataEnvio = DateTime.UtcNow;
        IsAtiva = true;
        IsGeral = isGeral;
        SendEmail = sendEmail;
        Leituras = new List<NotificacaoLeitura>();
    }

    public void Atualizar(string titulo, string mensagem)
    {
        Titulo = titulo;
        Mensagem = mensagem;
        SetUpdatedAt();
    }

    public void MarkAsRead(Guid userId)
    {
        if (!Leituras.Any(l => l.UserId == userId))
        {
            Leituras.Add(new NotificacaoLeitura(Id, userId));
        }
    }

    public void Activate() => IsAtiva = true;
    public void Deactivate() => IsAtiva = false;
    public void Ativar() => IsAtiva = true;
    public void Desativar() => IsAtiva = false;
}

public class NotificacaoLeitura : Entity
{
    public Guid NotificacaoId { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime DataLeitura { get; private set; }
    public Notificacao Notificacao { get; private set; } = null!;
    public User User { get; private set; } = null!;

    private NotificacaoLeitura() { }

    public NotificacaoLeitura(Guid notificacaoId, Guid userId)
    {
        NotificacaoId = notificacaoId;
        UserId = userId;
        DataLeitura = DateTime.UtcNow;
    }
}
