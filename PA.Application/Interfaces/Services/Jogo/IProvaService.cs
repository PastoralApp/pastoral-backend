using PA.Application.DTOs;

namespace PA.Application.Interfaces.Services;

public interface IProvaService
{
    Task<ProvaDto?> GetByIdAsync(Guid id);
    Task<ProvaDto?> GetByIdWithPontuacoesAsync(Guid id);
    Task<IEnumerable<ProvaDto>> GetAllAsync();
    Task<IEnumerable<ProvaDto>> GetByGuiaIdAsync(Guid guiaId);
    Task<ProvaDto> CreateAsync(CreateProvaDto dto);
    Task<ProvaDto> UpdateAsync(Guid id, UpdateProvaDto dto);
    Task DeleteAsync(Guid id);
    Task<PontuacaoProvaDto> RegistrarPontuacaoAsync(RegistrarPontuacaoProvaDto dto, Guid userId);
    Task<bool> FinalizarProvaAsync(Guid provaId);
    Task<IEnumerable<PontuacaoProvaDto>> GetPontuacoesByProvaAsync(Guid provaId);
}
