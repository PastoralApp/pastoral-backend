using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PA.Application.DTOs;
using PA.Application.Interfaces.Services;
using PA.Domain.Enums;
using System.Security.Claims;

namespace PA.API.Controllers.Post;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PostsController : ControllerBase
{
    private readonly IPostService _postService;

    public PostsController(IPostService postService)
    {
        _postService = postService;
    }

    #region Queries

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = GetCurrentUserId();
        var posts = await _postService.GetRecentAsync(100, userId);
        return Ok(posts);
    }

    [HttpGet("recent")]
    public async Task<IActionResult> GetRecent([FromQuery] int count = 50)
    {
        var userId = GetCurrentUserId();
        var posts = await _postService.GetRecentAsync(count, userId);
        return Ok(posts);
    }

    [HttpGet("pinned")]
    public async Task<IActionResult> GetPinned()
    {
        var posts = await _postService.GetPinnedAsync();
        return Ok(posts);
    }

    [HttpGet("pastoral/{pastoralId:guid}")]
    public async Task<IActionResult> GetByPastoral(Guid pastoralId)
    {
        var posts = await _postService.GetByPastoralAsync(pastoralId);
        return Ok(posts);
    }

    [HttpGet("tipo-pastoral/{tipoPastoral}")]
    public async Task<IActionResult> GetByTipoPastoral(string tipoPastoral)
    {
        if (!Enum.TryParse<TipoPastoral>(tipoPastoral, true, out var tipo))
            return BadRequest(new { message = "Tipo de pastoral inválido. Use: PA, PJ ou Geral" });

        var posts = await _postService.GetByTipoPastoralAsync(tipo);
        return Ok(posts);
    }

    [HttpGet("grupo/{grupoId:guid}")]
    public async Task<IActionResult> GetByGrupo(Guid grupoId)
    {
        var posts = await _postService.GetByGrupoAsync(grupoId);
        return Ok(posts);
    }

    [HttpGet("user/{userId:guid}")]
    public async Task<IActionResult> GetByUser(Guid userId)
    {
        var posts = await _postService.GetByUserAsync(userId);
        return Ok(posts);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var userId = GetCurrentUserId();
        var post = await _postService.GetByIdAsync(id, userId);
        
        if (post == null)
            return NotFound();

        return Ok(post);
    }

    [HttpGet("{id:guid}/detail")]
    public async Task<IActionResult> GetPostDetail(Guid id)
    {
        var userId = GetCurrentUserId();
        var post = await _postService.GetPostDetailAsync(id, userId);
        
        if (post == null)
            return NotFound();

        return Ok(post);
    }

    [HttpGet("saved")]
    public async Task<IActionResult> GetSavedPosts()
    {
        var userId = GetCurrentUserId();
        var savedPosts = await _postService.GetSavedPostsAsync(userId);
        return Ok(savedPosts);
    }

    [HttpGet("anuncios")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAnuncios([FromQuery] int count = 10)
    {
        var posts = await _postService.GetByTypeAsync(PostType.Anuncio, count);
        return Ok(posts);
    }

    [HttpGet("avisos")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAvisos([FromQuery] int count = 10)
    {
        var posts = await _postService.GetByTypeAsync(PostType.Aviso, count);
        return Ok(posts);
    }

    #endregion

    #region Commands

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePostDto dto)
    {
        var userId = GetCurrentUserId();
        var post = await _postService.CreateAsync(dto, userId);
        
        return CreatedAtAction(nameof(GetById), new { id = post.Id }, post);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePostDto dto)
    {
        try
        {
            await _postService.UpdateAsync(id, dto);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _postService.DeleteAsync(id);
        return NoContent();
    }

    [HttpPost("{id:guid}/pin")]
    [Authorize(Roles = "Coordenador de Grupo,Coordenador Geral,Administrador")]
    public async Task<IActionResult> Pin(Guid id, [FromBody] PinPostRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            await _postService.PinAsync(id, userId, request.PinType ?? "Geral");
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { message = ex.Message });
        }
    }

    [HttpPost("{id:guid}/unpin")]
    [Authorize(Roles = "Coordenador de Grupo,Coordenador Geral,Administrador")]
    public async Task<IActionResult> Unpin(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            await _postService.UnpinAsync(id, userId);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { message = ex.Message });
        }
    }

    #endregion

    #region Reactions

    [HttpPost("{id:guid}/react")]
    public async Task<IActionResult> React(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _postService.ToggleReactionAsync(id, userId);
            return Ok(new { reacted = result.reacted, likesCount = result.likesCount });
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    #endregion

    #region Comments

    [HttpGet("{id:guid}/comments")]
    public async Task<IActionResult> GetComments(Guid id)
    {
        try
        {
            var comments = await _postService.GetCommentsAsync(id);
            return Ok(comments);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost("{id:guid}/comments")]
    public async Task<IActionResult> AddComment(Guid id, [FromBody] CreateCommentDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var comment = await _postService.AddCommentAsync(id, userId, dto.Conteudo);
            return Ok(comment);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("comments/{commentId:guid}")]
    public async Task<IActionResult> DeleteComment(Guid commentId)
    {
        try
        {
            var userId = GetCurrentUserId();
            await _postService.DeleteCommentAsync(commentId, userId);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    #endregion

    #region Share & Save

    [HttpPost("{id:guid}/share")]
    public async Task<IActionResult> Share(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var sharesCount = await _postService.ShareAsync(id, userId);
            return Ok(new { sharesCount });
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost("{id:guid}/save")]
    public async Task<IActionResult> SavePost(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var saved = await _postService.ToggleSavePostAsync(id, userId);
            return Ok(new { saved });
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    #endregion

    #region Admin

    [HttpGet("admin/all")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> GetAllPostsAdmin()
    {
        var posts = await _postService.GetRecentAsync(1000);
        return Ok(posts);
    }

    [HttpGet("admin/user/{userId:guid}")]
    [Authorize(Roles = "Administrador,Coordenador Geral")]
    public async Task<IActionResult> GetPostsByUserAdmin(Guid userId)
    {
        var posts = await _postService.GetByUserAsync(userId);
        return Ok(posts);
    }

    [HttpDelete("admin/{id:guid}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> AdminDeletePost(Guid id)
    {
        await _postService.DeleteAsync(id);
        return NoContent();
    }

    [HttpPut("admin/{id:guid}/type")]
    [Authorize(Roles = "Administrador,Coordenador Geral")]
    public async Task<IActionResult> AdminChangePostType(Guid id, [FromBody] ChangePostTypeDto dto)
    {
        try
        {
            var postType = Enum.Parse<PostType>(dto.Type);
            await _postService.ChangeTypeAsync(id, postType);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ArgumentException)
        {
            return BadRequest(new { message = "Tipo de post inválido" });
        }
    }

    #endregion

    #region Private Methods

    private Guid GetCurrentUserId()
    {
        return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }

    #endregion
}

public record PinPostRequest(string? PinType);
