using PA.Domain.Entities;
using PA.Domain.Interfaces;

namespace PA.Application.Interfaces.Repositories;

/// <summary>
/// Interface de repositório para User
/// </summary>
public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetByGrupoIdAsync(Guid grupoId);
    Task<IEnumerable<User>> GetByRoleIdAsync(Guid roleId);
    Task<bool> EmailExistsAsync(string email);
}
