using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PA.Application.DTOs;
using PA.Application.Services;
using System.Security.Claims;

namespace PA.API.Controllers;

/// <summary>
/// Controller de postagens
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PostsController : ControllerBase
{
    private readonly PostService _postService;

    public PostsController(PostService postService)
    {
        _postService = postService;
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
}
