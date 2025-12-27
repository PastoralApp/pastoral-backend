using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PA.Application.DTOs;
using PA.Application.Interfaces.Services;
using PA.Application.Interfaces.Repositories;
using PA.Domain.ValueObjects;
using PA.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace PA.API.Controllers.User;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IUserRepository _userRepository;
    private readonly IGrupoService _grupoService;
    private readonly PastoralAppDbContext _context;

    public UsersController(IUserService userService, IUserRepository userRepository, IGrupoService grupoService, PastoralAppDbContext context)
    {
        _userService = userService;
        _userRepository = userRepository;
        _grupoService = grupoService;
        _context = context;
    }

    [HttpGet]
    [Authorize(Roles = "Administrador,Coordenador Geral")]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userService.GetAllAsync();
        return Ok(users);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var user = await _userService.GetByIdAsync(id);
        
        if (user == null)
            return NotFound();

        return Ok(user);
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var user = await _userService.GetByIdAsync(userId);
        if (user == null)
            return NotFound();

        return Ok(user);
    }

    [HttpGet("grupo/{grupoId:guid}")]
    public async Task<IActionResult> GetByGrupo(Guid grupoId)
    {
        var users = await _userService.GetByGrupoAsync(grupoId);
        return Ok(users);
    }

    [HttpPost]
    [Authorize(Roles = "Administrador,Coordenador Geral")]
    public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
    {
        try
        {
            var user = await _userService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Administrador,Coordenador Geral")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserDto dto)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            var email = new Email(dto.Email);
            user.UpdateProfile(dto.Name, dto.BirthDate, dto.PhotoUrl);
            user.UpdateEmail(email);
            user.UpdateRole(dto.RoleId);

            if (dto.IsActive)
                user.Activate();
            else
                user.Deactivate();

            await _userRepository.UpdateAsync(user);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPut("me/profile")]
    public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateUserProfileDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return NotFound();

        user.UpdateProfile(dto.Name, dto.BirthDate, dto.PhotoUrl);
        await _userRepository.UpdateAsync(user);

        return NoContent();
    }

    [HttpPatch("{id:guid}/desativar")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Desativar(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            return NotFound();

        user.Deactivate();
        await _userRepository.UpdateAsync(user);

        return NoContent();
    }

    [HttpPatch("{id:guid}/ativar")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Ativar(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            return NotFound();

        user.Activate();
        await _userRepository.UpdateAsync(user);

        return NoContent();
    }

    [HttpPost("{userId:guid}/grupos/{grupoId:guid}")]
    [Authorize(Roles = "Administrador,Coordenador Geral")]
    public async Task<IActionResult> AddToGrupo(Guid userId, Guid grupoId)
    {
        try
        {
            await _grupoService.AddMemberAsync(grupoId, userId);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{userId:guid}/grupos/{grupoId:guid}")]
    [Authorize(Roles = "Administrador,Coordenador Geral")]
    public async Task<IActionResult> RemoveFromGrupo(Guid userId, Guid grupoId)
    {
        try
        {
            await _grupoService.RemoveMemberAsync(grupoId, userId);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return NotFound(new { message = "Usuário não encontrado" });

            foreach (var userGrupo in user.UserGrupos.ToList())
            {
                _context.Set<PA.Domain.Entities.UserGrupo>().Remove(userGrupo);
            }

            await _userService.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Erro ao excluir usuário. Verifique se não há dependências." });
        }
    }

    [HttpPatch("{id:guid}/role")]
    [Authorize(Roles = "Administrador,Coordenador Geral")]
    public async Task<IActionResult> UpdateRole(Guid id, [FromBody] PA.Application.DTOs.UpdateUserRoleDto dto)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            return NotFound();

        var roleExists = await _context.Roles.AnyAsync(r => r.Id == dto.RoleId);
        if (!roleExists)
            return BadRequest(new { message = "Role not found" });

        user.UpdateRole(dto.RoleId);
        await _userRepository.UpdateAsync(user);

        return NoContent();
    }

    [HttpPut("admin/{id:guid}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> UpdateAdmin(Guid id, [FromBody] UpdateUserAdminDto dto)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            return NotFound();

        var roleExists = await _context.Roles.AnyAsync(r => r.Id == dto.RoleId);
        if (!roleExists)
            return BadRequest(new { message = "Role não encontrada" });

        user.UpdateProfile(dto.Name, dto.BirthDate, null);
        user.UpdateRole(dto.RoleId);
        
        await _userRepository.UpdateAsync(user);

        return NoContent();
    }

    [HttpPost("admin")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> CreateAdmin([FromBody] CreateUserDto dto)
    {
        try
        {
            var userDto = await _userService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = userDto.Id }, userDto);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

