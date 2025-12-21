using PA.Domain.Common;
using PA.Domain.Enums;

namespace PA.Domain.Entities;

/// <summary>
/// Representa um Post (postagem) no feed
/// </summary>
public class Post : AggregateRoot
{
    public string Content { get; private set; }
    public string? ImageUrl { get; private set; }
    public PostType Type { get; private set; }
    public bool IsPinned { get; private set; }
    public int LikesCount { get; private set; }
    public Guid AuthorId { get; private set; }

    // Navigation
    public User Author { get; private set; } = null!;

    private Post() 
    { 
        Content = string.Empty;
    }

    public Post(string content, Guid authorId, PostType type = PostType.Comum, string? imageUrl = null)
    {
        Content = content;
        AuthorId = authorId;
        Type = type;
        ImageUrl = imageUrl;
        IsPinned = false;
        LikesCount = 0;
    }

    public void UpdateContent(string content, string? imageUrl = null)
    {
        Content = content;
        ImageUrl = imageUrl;
        SetUpdatedAt();
    }

    public void Pin() => IsPinned = true;
    public void Unpin() => IsPinned = false;

    public void IncrementLikes() => LikesCount++;
    public void DecrementLikes()
    {
        if (LikesCount > 0)
            LikesCount--;
    }

    public void ChangeType(PostType type)
    {
        Type = type;
        SetUpdatedAt();
    }
}
