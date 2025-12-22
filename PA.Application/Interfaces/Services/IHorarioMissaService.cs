using PA.Application.DTOs;

namespace PA.Application.Interfaces.Services;

public interface IHorarioMissaService
{
    Task<HorarioMissaDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<HorarioMissaDto>> GetAllAsync(bool incluirInativos = false);
    Task<IEnumerable<HorarioMissaDto>> GetByIgrejaAsync(Guid igrejaId, bool incluirInativos = false);
    Task<IEnumerable<HorarioMissaDto>> GetByDiaSemanaAsync(DayOfWeek diaSemana, bool incluirInativos = false);
    Task<HorarioMissaDto> CreateAsync(CreateHorarioMissaDto dto);
    Task UpdateAsync(Guid id, CreateHorarioMissaDto dto);
    Task DeleteAsync(Guid id);
}
