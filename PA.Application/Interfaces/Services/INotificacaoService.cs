using PA.Application.DTOs;

namespace PA.Application.Interfaces.Services;

public interface INotificacaoService
{
    Task<NotificacaoDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<NotificacaoDto>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<NotificacaoDto>> GetUnreadAsync(Guid userId);
    Task<IEnumerable<NotificacaoDto>> GetByGrupoIdAsync(Guid grupoId);
    Task<NotificacaoDto> CreateAsync(CreateNotificacaoDto dto);
    Task UpdateAsync(Guid id, CreateNotificacaoDto dto);
    Task DeleteAsync(Guid id);
    Task MarkAsReadAsync(Guid id);
    Task DesativarAsync(Guid id);
    Task AtivarAsync(Guid id);
}
