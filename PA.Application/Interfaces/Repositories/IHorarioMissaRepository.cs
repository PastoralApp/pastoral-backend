using PA.Domain.Entities;

namespace PA.Application.Interfaces.Repositories;

public interface IHorarioMissaRepository
{
    Task<HorarioMissa?> GetByIdAsync(Guid id);
    Task<IEnumerable<HorarioMissa>> GetAllAsync(bool incluirInativos = false);
    Task<IEnumerable<HorarioMissa>> GetByIgrejaAsync(Guid igrejaId, bool incluirInativos = false);
    Task<IEnumerable<HorarioMissa>> GetByDiaSemanaAsync(DayOfWeek diaSemana, bool incluirInativos = false);
    Task<HorarioMissa> AddAsync(HorarioMissa horarioMissa);
    Task<HorarioMissa> UpdateAsync(HorarioMissa horarioMissa);
    Task DeleteAsync(Guid id);
}
