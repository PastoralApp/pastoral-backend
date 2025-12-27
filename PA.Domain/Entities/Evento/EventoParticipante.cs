using PA.Domain.Common;

namespace PA.Domain.Entities;

public class EventoParticipante : Entity
{
    public Guid EventoId { get; private set; }
    public Evento Evento { get; private set; } = null!;
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;
    public bool Confirmado { get; private set; }
    public DateTime? DataConfirmacao { get; private set; }

    private EventoParticipante() { }

    public EventoParticipante(Guid eventoId, Guid userId)
    {
        EventoId = eventoId;
        UserId = userId;
        Confirmado = true;
        DataConfirmacao = DateTime.UtcNow;
    }

    public void Confirmar()
    {
        Confirmado = true;
        DataConfirmacao = DateTime.UtcNow;
        SetUpdatedAt();
    }

    public void Cancelar()
    {
        Confirmado = false;
        DataConfirmacao = null;
        SetUpdatedAt();
    }
}
