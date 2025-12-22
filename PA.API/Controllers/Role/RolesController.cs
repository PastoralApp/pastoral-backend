using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PA.Application.DTOs;
using PA.Infrastructure.Data.Context;

namespace PA.API.Controllers.Role;

[ApiController]
[Route("api/[controller]")]
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
    public IActionResult GetAll()
    {
        var roles = _context.Roles.ToList();
        var dtos = _mapper.Map<List<RoleDto>>(roles);
        return Ok(dtos);
    }
}
