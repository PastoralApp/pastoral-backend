using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PA.Application.DTOs;
using PA.Application.Interfaces.Repositories;
using PA.Domain.Entities;
using PA.API.Filters;

namespace PA.API.Controllers.Tag;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TagsController : ControllerBase
{
    private readonly ITagRepository _tagRepository;
    private readonly IUserRepository _userRepository;

    public TagsController(ITagRepository tagRepository, IUserRepository userRepository)
    {
        _tagRepository = tagRepository;
        _userRepository = userRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var tags = await _tagRepository.GetAllAsync();
        var dtos = tags.Select(t => new TagDto
        {
            Id = t.Id,
            Name = t.Name,
            Color = t.Color,
            Description = t.Description
        });

        return Ok(dtos);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var tag = await _tagRepository.GetByIdAsync(id);
        if (tag == null)
            return NotFound();

        var dto = new TagDto
        {
            Id = tag.Id,
            Name = tag.Name,
            Color = tag.Color,
            Description = tag.Description
        };

        return Ok(dto);
    }

    [HttpPost]
    [Authorize(Roles = "Administrador,Coordenador Geral")]
    [ServiceFilter(typeof(AuthorizationFilter))]
    public async Task<IActionResult> Create([FromBody] CreateTagDto dto)
    {
        var tag = new PA.Domain.Entities.Tag(dto.Name, dto.Color, dto.Description);
        var created = await _tagRepository.AddAsync(tag);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, new TagDto
        {
            Id = created.Id,
            Name = created.Name,
            Color = created.Color,
            Description = created.Description
        });
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Administrador,Coordenador Geral")]
    [ServiceFilter(typeof(AuthorizationFilter))]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateTagDto dto)
    {
        var tag = await _tagRepository.GetByIdAsync(id);
        if (tag == null)
            return NotFound();

        tag.UpdateInfo(dto.Name, dto.Color, dto.Description);
        await _tagRepository.UpdateAsync(tag);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Administrador,Coordenador Geral")]
    [ServiceFilter(typeof(AuthorizationFilter))]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _tagRepository.DeleteAsync(id);
        return NoContent();
    }

    [HttpPost("{id:guid}/users/{userId:guid}")]
    [Authorize(Roles = "Administrador,Coordenador Geral")]
    [ServiceFilter(typeof(AuthorizationFilter))]
    public async Task<IActionResult> AddTagToUser(Guid id, Guid userId)
    {
        var tag = await _tagRepository.GetByIdAsync(id);
        if (tag == null)
            return NotFound("Tag não encontrada");

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return NotFound("Usuário não encontrado");

        user.AddTag(tag);
        await _userRepository.UpdateAsync(user);

        return NoContent();
    }

    [HttpDelete("{id:guid}/users/{userId:guid}")]
    [Authorize(Roles = "Administrador,Coordenador Geral")]
    [ServiceFilter(typeof(AuthorizationFilter))]
    public async Task<IActionResult> RemoveTagFromUser(Guid id, Guid userId)
    {
        var tag = await _tagRepository.GetByIdAsync(id);
        if (tag == null)
            return NotFound("Tag não encontrada");

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return NotFound("Usuário não encontrado");

        user.RemoveTag(tag);
        await _userRepository.UpdateAsync(user);

        return NoContent();
    }
}
