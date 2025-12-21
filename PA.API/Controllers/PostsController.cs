using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PA.Application.DTOs;
using PA.Application.Services;
using PA.Application.Interfaces.Repositories;
using PA.Domain.Entities;
using System.Security.Claims;

namespace PA.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PostsController : ControllerBase
{
    private readonly PostService _postService;
    private readonly IPostRepository _postRepository;

    public PostsController(PostService postService, IPostRepository postRepository)
    {
        _postService = postService;
        _postRepository = postRepository;
    }

    [HttpGet("recent")]
    public async Task<IActionResult> GetRecent([FromQuery] int count = 50)
    {
        var posts = await _postService.GetRecentAsync(count);
        return Ok(posts);
    }

    [HttpGet("pinned")]
    public async Task<IActionResult> GetPinned()
    {
        var posts = await _postService.GetPinnedAsync();
        return Ok(posts);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var post = await _postService.GetByIdAsync(id);
        
        if (post == null)
            return NotFound();

        return Ok(post);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePostDto dto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
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
    [Authorize(Roles = "CoordenadorGeral,Admin")]
    public async Task<IActionResult> Pin(Guid id)
    {
        try
        {
            await _postService.PinAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost("{id:guid}/unpin")]
    [Authorize(Roles = "CoordenadorGeral,Admin")]
    public async Task<IActionResult> Unpin(Guid id)
    {
        try
        {
            await _postService.UnpinAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost("{id:guid}/react")]
    public async Task<IActionResult> React(Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var post = await _postRepository.GetByIdAsync(id);
        
        if (post == null)
            return NotFound();

        var existingReaction = post.Reactions.FirstOrDefault(r => r.UserId == userId);
        
        if (existingReaction != null)
        {
            post.Reactions.Remove(existingReaction);
            post.DecrementLikes();
        }
        else
        {
            post.Reactions.Add(new PostReaction(id, userId));
            post.IncrementLikes();
        }

        await _postRepository.UpdateAsync(post);
        return Ok(new { reacted = existingReaction == null, likesCount = post.LikesCount });
    }

    [HttpPost("{id:guid}/comments")]
    public async Task<IActionResult> AddComment(Guid id, [FromBody] CreateCommentDto dto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var post = await _postRepository.GetByIdAsync(id);
        
        if (post == null)
            return NotFound();

        var comment = new PostComment(id, userId, dto.Conteudo);
        post.Comments.Add(comment);
        await _postRepository.UpdateAsync(post);

        return Ok(comment);
    }

    [HttpGet("{id:guid}/comments")]
    public async Task<IActionResult> GetComments(Guid id)
    {
        var post = await _postRepository.GetByIdAsync(id);
        
        if (post == null)
            return NotFound();

        var comments = post.Comments.Where(c => c.IsAtivo).OrderBy(c => c.DataComentario);
        return Ok(comments);
    }

    [HttpPost("{id:guid}/share")]
    public async Task<IActionResult> Share(Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var post = await _postRepository.GetByIdAsync(id);
        
        if (post == null)
            return NotFound();

        var share = new PostShare(id, userId);
        post.Shares.Add(share);
        await _postRepository.UpdateAsync(post);

        return Ok(new { sharesCount = post.Shares.Count });
    }

    [HttpDelete("comments/{commentId:guid}")]
    public async Task<IActionResult> DeleteComment(Guid commentId)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var isAdmin = User.IsInRole("Admin") || User.IsInRole("CoordenadorGeral");

        var comment = await _postRepository.FindAsync(p => p.Comments.Any(c => c.Id == commentId));
        var commentToDelete = comment.FirstOrDefault()?.Comments.FirstOrDefault(c => c.Id == commentId);

        if (commentToDelete == null)
            return NotFound();

        if (commentToDelete.UserId != userId && !isAdmin)
            return Forbid();

        commentToDelete.Desativar();
        await _postRepository.UpdateAsync(comment.First());

        return NoContent();
    }


    [HttpPost("{id:guid}/save")]
    public async Task<IActionResult> SavePost(Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var post = await _postRepository.GetByIdAsync(id);
        
        if (post == null)
            return NotFound();

        var saved = await _postRepository.GetPostSalvoAsync(id, userId);

        if (saved != null)
        {
            await _postRepository.RemovePostSalvoAsync(saved);
            return Ok(new { saved = false });
        }
        else
        {
            var postSalvo = new PostSalvo(id, userId);
            await _postRepository.AddPostSalvoAsync(postSalvo);
            return Ok(new { saved = true });
        }
    }

    [HttpGet("saved")]
    public async Task<IActionResult> GetSavedPosts()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var savedPosts = await _postRepository.GetSavedPostsByUserAsync(userId);
        return Ok(savedPosts);
    }
}
