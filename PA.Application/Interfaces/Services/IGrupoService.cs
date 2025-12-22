using PA.Application.DTOs;

namespace PA.Application.Interfaces.Services;

public interface IGrupoService
{
    Task<GrupoDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<GrupoDto>> GetAllAsync(bool incluirInativos = false);
    Task<IEnumerable<GrupoDto>> GetByPastoralIdAsync(Guid pastoralId, bool incluirInativos = false);
    Task<GrupoDto> CreateAsync(CreateGrupoDto dto);
    Task UpdateAsync(Guid id, CreateGrupoDto dto);
    Task DeleteAsync(Guid id);
    Task DesativarAsync(Guid id);
    Task AtivarAsync(Guid id);
    Task AddMemberAsync(Guid grupoId, Guid userId);
    Task RemoveMemberAsync(Guid grupoId, Guid userId);
}
