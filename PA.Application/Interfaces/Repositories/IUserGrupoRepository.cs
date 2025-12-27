using PA.Domain.Entities;

namespace PA.Application.Interfaces.Repositories;

public interface IUserGrupoRepository
{
    Task<UserGrupo?> GetByUserAndGrupoAsync(Guid userId, Guid grupoId);
    Task<IEnumerable<UserGrupo>> GetByUserIdAsync(Guid userId, bool apenasAtivos = true);
    Task<IEnumerable<UserGrupo>> GetByGrupoIdAsync(Guid grupoId, bool apenasAtivos = true);
    Task<UserGrupo> AddAsync(UserGrupo userGrupo);
    Task UpdateAsync(UserGrupo userGrupo);
    Task<int> CountActiveGruposByUserAsync(Guid userId);
}
