namespace PA.Application.DTOs;

public class CreateCommentDto
{
    public string Conteudo { get; set; } = string.Empty;
}

public class CommentDto
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
    public string? UserName { get; set; }
    public string? UserPhotoUrl { get; set; }
    public string Conteudo { get; set; } = string.Empty;
    public DateTime DataComentario { get; set; }
    public bool IsAtivo { get; set; }
}
