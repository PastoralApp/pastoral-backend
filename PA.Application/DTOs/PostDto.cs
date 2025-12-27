namespace PA.Application.DTOs;

public class PostDto
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string Type { get; set; } = string.Empty;
    public string TipoPastoral { get; set; } = string.Empty;
    public Guid? PastoralId { get; set; }
    public bool IsPinned { get; set; }
    public string? PinType { get; set; }
    public int LikesCount { get; set; }
    public int CommentsCount { get; set; }
    public bool IsLiked { get; set; }
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string? AuthorPhotoUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreatePostDto
{
    public string Content { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string Type { get; set; } = "Comum";
    public string TipoPastoral { get; set; } = "Geral";
    public Guid? PastoralId { get; set; }
}

public class UpdatePostDto
{
    public string Content { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
}

public class ChangePostTypeDto
{
    public string Type { get; set; } = string.Empty;
}

public class PostDetailDto
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string Type { get; set; } = string.Empty;
    public string TipoPastoral { get; set; } = string.Empty;
    public Guid? PastoralId { get; set; }
    public bool IsPinned { get; set; }
    public string? PinType { get; set; }
    public int LikesCount { get; set; }
    public int CommentsCount { get; set; }
    public int SharesCount { get; set; }
    public bool IsLiked { get; set; }
    public bool IsSaved { get; set; }
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string? AuthorPhotoUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<CommentDto> Comments { get; set; } = new();
}
