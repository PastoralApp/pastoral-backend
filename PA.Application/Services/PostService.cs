using AutoMapper;
using PA.Application.DTOs;
using PA.Application.Interfaces.Repositories;
using PA.Application.Interfaces.Services;
using PA.Domain.Entities;
using PA.Domain.Enums;

namespace PA.Application.Services;

public class PostService : IPostService
{
    private readonly IPostRepository _postRepository;
    private readonly IUserRepository _userRepository;
    private readonly INotificacaoService _notificacaoService;
    private readonly IMapper _mapper;

    public PostService(IPostRepository postRepository, IUserRepository userRepository, INotificacaoService notificacaoService, IMapper mapper)
    {
        _postRepository = postRepository;
        _userRepository = userRepository;
        _notificacaoService = notificacaoService;
        _mapper = mapper;
    }

    public async Task<PostDto?> GetByIdAsync(Guid id)
    {
        var post = await _postRepository.GetByIdAsync(id);
        return _mapper.Map<PostDto>(post);
    }

    public async Task<PostDto?> GetByIdAsync(Guid id, Guid currentUserId)
    {
        var post = await _postRepository.GetByIdAsync(id);
        if (post == null) return null;

        var postDto = _mapper.Map<PostDto>(post);
        postDto.IsLiked = await _postRepository.GetReactionAsync(id, currentUserId) != null;
        return postDto;
    }

    public async Task<PostDetailDto?> GetPostDetailAsync(Guid id, Guid currentUserId)
    {
        var post = await _postRepository.GetByIdAsync(id);
        if (post == null) return null;

        var postDto = _mapper.Map<PostDetailDto>(post);
        postDto.IsLiked = await _postRepository.GetReactionAsync(id, currentUserId) != null;
        postDto.IsSaved = await _postRepository.GetPostSalvoAsync(id, currentUserId) != null;
        
        var comments = await _postRepository.GetCommentsByPostIdAsync(id);
        postDto.Comments = _mapper.Map<List<CommentDto>>(comments);
        
        return postDto;
    }

    public async Task<IEnumerable<PostDto>> GetRecentAsync(int count = 50)
    {
        var posts = await _postRepository.GetRecentPostsAsync(count);
        return _mapper.Map<IEnumerable<PostDto>>(posts);
    }

    public async Task<IEnumerable<PostDto>> GetRecentAsync(int count, Guid currentUserId)
    {
        var posts = await _postRepository.GetRecentPostsAsync(count);
        return await MapPostsWithLikes(posts, currentUserId);
    }

    public async Task<IEnumerable<PostDto>> GetPinnedAsync()
    {
        var posts = await _postRepository.GetPinnedPostsAsync();
        return _mapper.Map<IEnumerable<PostDto>>(posts);
    }

    public async Task<IEnumerable<PostDto>> GetByPastoralAsync(Guid pastoralId)
    {
        var posts = await _postRepository.GetByPastoralAsync(pastoralId);
        return _mapper.Map<IEnumerable<PostDto>>(posts);
    }

    public async Task<IEnumerable<PostDto>> GetByTipoPastoralAsync(TipoPastoral tipoPastoral)
    {
        var posts = await _postRepository.GetByTipoPastoralAsync(tipoPastoral);
        return _mapper.Map<IEnumerable<PostDto>>(posts);
    }

    public async Task<IEnumerable<PostDto>> GetByGrupoAsync(Guid grupoId)
    {
        var posts = await _postRepository.GetByGrupoAsync(grupoId);
        return _mapper.Map<IEnumerable<PostDto>>(posts);
    }

    public async Task<IEnumerable<PostDto>> GetByUserAsync(Guid userId)
    {
        var posts = await _postRepository.GetByAuthorIdAsync(userId);
        return _mapper.Map<IEnumerable<PostDto>>(posts);
    }

    public async Task<PostDto> CreateAsync(CreatePostDto dto, Guid authorId)
    {
        var user = await _userRepository.GetByIdAsync(authorId);
        if (user == null)
            throw new KeyNotFoundException("Usuário não encontrado");

        var postType = Enum.Parse<PostType>(dto.Type);
        var tipoPastoral = Enum.Parse<TipoPastoral>(dto.TipoPastoral);

        var userRole = user.Role?.Name ?? "";
        var isAdmin = userRole == "Administrador";
        var isCoordenadorGeral = userRole == "Coordenador Geral";
        var isCoordenadorGrupo = userRole == "Coordenador de Grupo";
        
        var isAdminOrCoord = isAdmin || isCoordenadorGeral || isCoordenadorGrupo;
        var canCreateAnuncioAviso = isAdmin || isCoordenadorGeral;

        if ((postType == PostType.Anuncio || postType == PostType.Aviso) && !canCreateAnuncioAviso)
        {
            throw new UnauthorizedAccessException("Apenas administradores e coordenadores gerais podem criar anúncios e avisos");
        }

        if (!isAdminOrCoord && postType != PostType.Comum)
        {
            postType = PostType.Comum;
        }

        var pastoralId = dto.PastoralId;
        if (tipoPastoral == TipoPastoral.Geral)
        {
            pastoralId = null;
        }

        var post = new Post(dto.Content, authorId, postType, tipoPastoral, pastoralId, dto.ImageUrl);

        var created = await _postRepository.AddAsync(post);
        return _mapper.Map<PostDto>(created);
    }

    private async Task<List<PostDto>> MapPostsWithLikes(IEnumerable<Post> posts, Guid currentUserId)
    {
        var postDtos = _mapper.Map<List<PostDto>>(posts);
        
        foreach (var postDto in postDtos)
        {
            var reaction = await _postRepository.GetReactionAsync(postDto.Id, currentUserId);
            postDto.IsLiked = reaction != null;
        }

        return postDtos;
    }

    public async Task UpdateAsync(Guid id, UpdatePostDto dto)
    {
        var post = await _postRepository.GetByIdAsync(id);
        if (post == null)
            throw new KeyNotFoundException("Post não encontrado");

        post.UpdateContent(dto.Content, dto.ImageUrl);
        await _postRepository.UpdateAsync(post);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _postRepository.DeleteAsync(id);
    }

    public async Task PinAsync(Guid id, Guid userId, string pinType = "Geral")
    {
        var post = await _postRepository.GetByIdAsync(id);
        if (post == null)
            throw new KeyNotFoundException("Post não encontrado");

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new UnauthorizedAccessException("Usuário não encontrado");

        var userRole = user.Role?.Name ?? "";
        
        string finalPinType;
        if (userRole == "Administrador")
        {
            finalPinType = "Admin";
        }
        else if (userRole == "Coordenador Geral")
        {
            finalPinType = "Coordenador Geral";
        }
        else if (userRole == "Coordenador de Grupo")
        {
            var userGrupos = user.UserGrupos.Where(ug => ug.IsAtivo).Select(ug => ug.GrupoId).ToList();
            
            if (post.PastoralId.HasValue)
            {
                var hasPermission = userGrupos.Any();
                if (!hasPermission)
                    throw new UnauthorizedAccessException("Você não tem permissão para fixar posts desta pastoral");
            }
            
            finalPinType = "Coordenador de Grupo";
        }
        else
        {
            throw new UnauthorizedAccessException("Você não tem permissão para fixar posts");
        }

        post.Pin(finalPinType);
        await _postRepository.UpdateAsync(post);
    }

    public async Task UnpinAsync(Guid id, Guid userId)
    {
        var post = await _postRepository.GetByIdAsync(id);
        if (post == null)
            throw new KeyNotFoundException("Post não encontrado");

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new UnauthorizedAccessException("Usuário não encontrado");

        var userRole = user.Role?.Name ?? "";
        
        if (userRole != "Administrador")
        {
            if (userRole == "Coordenador Geral" && post.PinType == "Admin")
                throw new UnauthorizedAccessException("Apenas administradores podem desafixar posts fixados por administradores");
            
            if (userRole == "Coordenador de Grupo" && 
                (post.PinType == "Admin" || post.PinType == "Coordenador Geral"))
                throw new UnauthorizedAccessException("Você não tem permissão para desafixar este post");
        }

        post.Unpin();
        await _postRepository.UpdateAsync(post);
    }

    public async Task<(bool reacted, int likesCount)> ToggleReactionAsync(Guid postId, Guid userId)
    {
        var post = await _postRepository.GetByIdAsync(postId);
        if (post == null)
            throw new KeyNotFoundException("Post não encontrado");

        var existingReaction = await _postRepository.GetReactionAsync(postId, userId);

        if (existingReaction != null)
        {
            await _postRepository.RemoveReactionAsync(existingReaction);
            post.DecrementLikes();
        }
        else
        {
            var reaction = new PostReaction(postId, userId);
            await _postRepository.AddReactionAsync(reaction);
            post.IncrementLikes();
        }

        await _postRepository.UpdateAsync(post);
        return (existingReaction == null, post.LikesCount);
    }

    public async Task<CommentDto> AddCommentAsync(Guid postId, Guid userId, string conteudo)
    {
        var post = await _postRepository.GetByIdAsync(postId);
        if (post == null)
            throw new KeyNotFoundException("Post não encontrado");

        var comment = new PostComment(postId, userId, conteudo);
        var addedComment = await _postRepository.AddCommentAsync(comment);

        post.IncrementComments();
        await _postRepository.UpdateAsync(post);

        if (post.AuthorId != userId)
        {
            try
            {
                await _notificacaoService.CreateAsync(new CreateNotificacaoDto(
                    Titulo: "Novo comentário",
                    Mensagem: "Comentaram no seu post",
                    RemetenteId: userId,
                    GrupoId: null,
                    DestinatarioId: post.AuthorId,
                    IsGeral: false,
                    SendEmail: true
                ));
            }
            catch (Exception)
            {
            }
        }

        return new CommentDto
        {
            Id = addedComment.Id,
            PostId = addedComment.PostId,
            UserId = addedComment.UserId,
            UserName = addedComment.User?.Name,
            UserPhotoUrl = addedComment.User?.PhotoUrl,
            Conteudo = addedComment.Conteudo,
            DataComentario = addedComment.DataComentario,
            IsAtivo = addedComment.IsAtivo
        };
    }

    public async Task DeleteCommentAsync(Guid commentId, Guid userId)
    {
        var comment = await _postRepository.GetCommentByIdAsync(commentId);
        if (comment == null)
            throw new KeyNotFoundException("Comentário não encontrado");

        if (comment.UserId != userId)
            throw new UnauthorizedAccessException("Você não tem permissão para excluir este comentário");

        var post = await _postRepository.GetByIdAsync(comment.PostId);
        if (post != null)
        {
            post.DecrementComments();
            await _postRepository.UpdateAsync(post);
        }

        await _postRepository.RemoveCommentAsync(comment);
    }

    public async Task<IEnumerable<CommentDto>> GetCommentsAsync(Guid postId)
    {
        var comments = await _postRepository.GetCommentsByPostIdAsync(postId);
        return _mapper.Map<IEnumerable<CommentDto>>(comments);
    }

    public async Task<int> ShareAsync(Guid postId, Guid userId)
    {
        var post = await _postRepository.GetByIdAsync(postId);
        if (post == null)
            throw new KeyNotFoundException("Post não encontrado");

        var share = new PostShare(postId, userId);
        await _postRepository.AddShareAsync(share);

        return post.Shares.Count + 1;
    }

    public async Task<bool> ToggleSavePostAsync(Guid postId, Guid userId)
    {
        var post = await _postRepository.GetByIdAsync(postId);
        if (post == null)
            throw new KeyNotFoundException("Post não encontrado");

        var saved = await _postRepository.GetPostSalvoAsync(postId, userId);

        if (saved != null)
        {
            await _postRepository.RemovePostSalvoAsync(saved);
            return false;
        }
        else
        {
            var postSalvo = new PostSalvo(postId, userId);
            await _postRepository.AddPostSalvoAsync(postSalvo);
            return true;
        }
    }

    public async Task<IEnumerable<PostDto>> GetSavedPostsAsync(Guid userId)
    {
        var posts = await _postRepository.GetSavedPostsByUserAsync(userId);
        return _mapper.Map<IEnumerable<PostDto>>(posts);
    }

    public async Task ChangeTypeAsync(Guid postId, PostType newType)
    {
        var post = await _postRepository.GetByIdAsync(postId);
        if (post == null)
            throw new KeyNotFoundException("Post não encontrado");

        post.ChangeType(newType);
        await _postRepository.UpdateAsync(post);
    }

    public async Task<IEnumerable<PostDto>> GetByTypeAsync(PostType type, int count = 10)
    {
        var posts = await _postRepository.GetByTypeAsync(type, count);
        return _mapper.Map<IEnumerable<PostDto>>(posts);
    }
}
