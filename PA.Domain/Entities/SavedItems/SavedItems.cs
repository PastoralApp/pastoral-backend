using PA.Domain.Common;

namespace PA.Domain.Entities;

public class PostSalvo : Entity
{
    public Guid PostId { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime DataSalvamento { get; private set; }
    public Post Post { get; private set; } = null!;
    public User User { get; private set; } = null!;

    private PostSalvo() { }

    public PostSalvo(Guid postId, Guid userId)
    {
        PostId = postId;
        UserId = userId;
        DataSalvamento = DateTime.UtcNow;
    }
}

public class EventoSalvo : Entity
{
    public Guid EventoId { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime DataSalvamento { get; private set; }
    public Evento Evento { get; private set; } = null!;
    public User User { get; private set; } = null!;

    private EventoSalvo() { }

    public EventoSalvo(Guid eventoId, Guid userId)
    {
        EventoId = eventoId;
        UserId = userId;
        DataSalvamento = DateTime.UtcNow;
    }
}
