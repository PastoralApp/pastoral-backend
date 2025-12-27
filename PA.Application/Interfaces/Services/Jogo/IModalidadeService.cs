using PA.Application.DTOs;

namespace PA.Application.Interfaces.Services;

public interface IModalidadeService
{
    Task<ModalidadeDto?> GetByIdAsync(Guid id);
    Task<ModalidadeDto?> GetByIdWithPartidasAsync(Guid id);
    Task<IEnumerable<ModalidadeDto>> GetAllAsync();
    Task<IEnumerable<ModalidadeDto>> GetByOlimpiadasIdAsync(Guid olimpiadasId);
    Task<ModalidadeDto> CreateAsync(CreateModalidadeDto dto);
    Task<ModalidadeDto> UpdateAsync(Guid id, UpdateModalidadeDto dto);
    Task DeleteAsync(Guid id);
    Task<bool> InscreverGrupoAsync(Guid modalidadeId, Guid grupoId, List<Guid>? participantesIds = null);
    Task<bool> RemoverGrupoAsync(Guid modalidadeId, Guid grupoId);
    Task<bool> GerarChaveamentoAsync(Guid modalidadeId);
    Task<bool> AvancarFaseAsync(Guid modalidadeId);
}
