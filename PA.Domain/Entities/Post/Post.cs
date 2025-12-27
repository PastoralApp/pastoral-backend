using PA.Domain.Common;
using PA.Domain.Enums;

namespace PA.Domain.Entities;

public class Post : AggregateRoot
{
    public string Content { get; private set; }
    public string? ImageUrl { get; private set; }
    public PostType Type { get; private set; }
    public bool IsPinned { get; private set; }
    public string? PinType { get; private set; }
    public TipoPastoral TipoPastoral { get; private set; }
    public Guid? PastoralId { get; private set; }
    public int LikesCount { get; private set; }
    public int CommentsCount { get; private set; }
    public Guid AuthorId { get; private set; }
    public User Author { get; private set; } = null!;
    public ICollection<PostReaction> Reactions { get; private set; }
    public ICollection<PostComment> Comments { get; private set; }
    public ICollection<PostShare> Shares { get; private set; }

    private Post() 
    { 
        Content = string.Empty;
        Reactions = new List<PostReaction>();
        Comments = new List<PostComment>();
        Shares = new List<PostShare>();
    }

    public Post(string content, Guid authorId, PostType type = PostType.Comum, TipoPastoral tipoPastoral = TipoPastoral.Geral, Guid? pastoralId = null, string? imageUrl = null)
    {
        Content = content;
        AuthorId = authorId;
        Type = type;
        TipoPastoral = tipoPastoral;
        PastoralId = pastoralId;
        ImageUrl = imageUrl;
        IsPinned = false;
        LikesCount = 0;
        CommentsCount = 0;
        Reactions = new List<PostReaction>();
        Comments = new List<PostComment>();
        Shares = new List<PostShare>();
    }

    public void UpdateContent(string content, string? imageUrl = null)
    {
        Content = content;
        ImageUrl = imageUrl;
        SetUpdatedAt();
    }

    public void Pin(string pinType = "Geral") 
    {
        IsPinned = true;
        PinType = pinType;
    }
    
    public void Unpin() 
    {
        IsPinned = false;
        PinType = null;
    }

    public void IncrementLikes() => LikesCount++;
    public void DecrementLikes()
    {
        if (LikesCount > 0)
            LikesCount--;
    }

    public void IncrementComments() => CommentsCount++;
    public void DecrementComments()
    {
        if (CommentsCount > 0)
            CommentsCount--;
    }

    public void ChangeType(PostType type)
    {
        Type = type;
        SetUpdatedAt();
    }

    public void SetPastoral(TipoPastoral tipoPastoral, Guid? pastoralId = null)
    {
        TipoPastoral = tipoPastoral;
        PastoralId = pastoralId;
        SetUpdatedAt();
    }
}
