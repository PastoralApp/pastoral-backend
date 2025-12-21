using PA.Domain.Common;

namespace PA.Domain.Entities;

public class PostReaction : Entity
{
    public Guid PostId { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime DataReacao { get; private set; }
    public Post Post { get; private set; } = null!;
    public User User { get; private set; } = null!;

    private PostReaction() { }

    public PostReaction(Guid postId, Guid userId)
    {
        PostId = postId;
        UserId = userId;
        DataReacao = DateTime.UtcNow;
    }
}

public class PostComment : Entity
{
    public Guid PostId { get; private set; }
    public Guid UserId { get; private set; }
    public string Conteudo { get; private set; }
    public DateTime DataComentario { get; private set; }
    public bool IsAtivo { get; private set; }
    public Post Post { get; private set; } = null!;
    public User User { get; private set; } = null!;

    private PostComment() 
    { 
        Conteudo = string.Empty;
    }

    public PostComment(Guid postId, Guid userId, string conteudo)
    {
        PostId = postId;
        UserId = userId;
        Conteudo = conteudo;
        DataComentario = DateTime.UtcNow;
        IsAtivo = true;
    }

    public void Editar(string conteudo)
    {
        Conteudo = conteudo;
        SetUpdatedAt();
    }

    public void Desativar() => IsAtivo = false;
    public void Ativar() => IsAtivo = true;
}

public class PostShare : Entity
{
    public Guid PostId { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime DataCompartilhamento { get; private set; }
    public Post Post { get; private set; } = null!;
    public User User { get; private set; } = null!;

    private PostShare() { }

    public PostShare(Guid postId, Guid userId)
    {
        PostId = postId;
        UserId = userId;
        DataCompartilhamento = DateTime.UtcNow;
    }
}
