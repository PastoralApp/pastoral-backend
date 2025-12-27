using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PA.Application.DTOs;
using PA.Domain.Entities;
using PA.Domain.Enums;
using PA.Infrastructure.Data.Context;
using PA.API.Filters;

namespace PA.API.Controllers.Role;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RolesController : ControllerBase
{
    private readonly PastoralAppDbContext _context;
    private readonly IMapper _mapper;

    public RolesController(PastoralAppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var roles = await _context.Roles.ToListAsync();
        var dtos = _mapper.Map<List<RoleDto>>(roles);
        return Ok(dtos);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var role = await _context.Roles.FindAsync(id);
        if (role == null)
            return NotFound();

        var dto = _mapper.Map<RoleDto>(role);
        return Ok(dto);
    }

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    [ServiceFilter(typeof(AuthorizationFilter))]
    public async Task<IActionResult> Create([FromBody] CreateRoleDto dto)
    {
        var role = new PA.Domain.Entities.Role(dto.Name, (RoleType)dto.Type, dto.Description);
        _context.Roles.Add(role);
        await _context.SaveChangesAsync();

        var roleDto = _mapper.Map<RoleDto>(role);
        return CreatedAtAction(nameof(GetById), new { id = role.Id }, roleDto);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Administrador")]
    [ServiceFilter(typeof(AuthorizationFilter))]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateRoleDto dto)
    {
        var role = await _context.Roles.FindAsync(id);
        if (role == null)
            return NotFound();

        role.UpdateInfo(dto.Name, dto.Description);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Administrador")]
    [ServiceFilter(typeof(AuthorizationFilter))]
    public async Task<IActionResult> Delete(Guid id)
    {
        var role = await _context.Roles.FindAsync(id);
        if (role == null)
            return NotFound();

        _context.Roles.Remove(role);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
