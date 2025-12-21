using Microsoft.EntityFrameworkCore;
using PA.Application.Interfaces.Repositories;
using PA.Domain.Entities;
using PA.Infrastructure.Data.Context;

namespace PA.Infrastructure.Repositories;

/// <summary>
/// Implementação do repositório de User
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly PastoralAppDbContext _context;

    public UserRepository(PastoralAppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users
            .Include(u => u.Role)
            .Include(u => u.UserGrupos)
                .ThenInclude(ug => ug.Grupo)
            .Include(u => u.Tags)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Users
            .Include(u => u.Role)
            .Include(u => u.UserGrupos)
                .ThenInclude(ug => ug.Grupo)
            .Include(u => u.Tags)
            .ToListAsync();
    }

    public async Task<IEnumerable<User>> FindAsync(System.Linq.Expressions.Expression<Func<User, bool>> predicate)
    {
        return await _context.Users
            .Include(u => u.Role)
            .Include(u => u.UserGrupos)
                .ThenInclude(ug => ug.Grupo)
            .Where(predicate)
            .ToListAsync();
    }

    public async Task<User> AddAsync(User entity)
    {
        await _context.Users.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(User entity)
    {
        _context.Users.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var user = await GetByIdAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Users.AnyAsync(u => u.Id == id);
    }

    public async Task<int> CountAsync()
    {
        return await _context.Users.CountAsync();
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .Include(u => u.Role)
            .Include(u => u.UserGrupos)
                .ThenInclude(ug => ug.Grupo)
            .FirstOrDefaultAsync(u => u.Email.Value == email);
    }

    public async Task<IEnumerable<User>> GetByGrupoIdAsync(Guid grupoId)
    {
        return await _context.Users
            .Include(u => u.Role)
            .Include(u => u.UserGrupos)
                .ThenInclude(ug => ug.Grupo)
            .Where(u => u.UserGrupos.Any(ug => ug.GrupoId == grupoId && ug.IsAtivo))
            .ToListAsync();
    }

    public async Task<IEnumerable<User>> GetByRoleIdAsync(Guid roleId)
    {
        return await _context.Users
            .Include(u => u.Role)
            .Include(u => u.UserGrupos)
                .ThenInclude(ug => ug.Grupo)
            .Where(u => u.RoleId == roleId)
            .ToListAsync();
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.Users.AnyAsync(u => u.Email.Value == email);
    }
}
