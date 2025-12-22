using AutoMapper;
using PA.Application.DTOs;
using PA.Application.Interfaces.Repositories;
using PA.Application.Interfaces.Services;

namespace PA.Application.Services;

public class HorarioMissaService : IHorarioMissaService
{
    private readonly IHorarioMissaRepository _horarioMissaRepository;
    private readonly IIgrejaRepository _igrejaRepository;
    private readonly IMapper _mapper;

    public HorarioMissaService(
        IHorarioMissaRepository horarioMissaRepository,
        IIgrejaRepository igrejaRepository,
        IMapper mapper)
    {
        _horarioMissaRepository = horarioMissaRepository;
        _igrejaRepository = igrejaRepository;
        _mapper = mapper;
    }

    public async Task<HorarioMissaDto?> GetByIdAsync(Guid id)
    {
        var horario = await _horarioMissaRepository.GetByIdAsync(id);
        return _mapper.Map<HorarioMissaDto>(horario);
    }

    public async Task<IEnumerable<HorarioMissaDto>> GetAllAsync(bool incluirInativos = false)
    {
        var horarios = await _horarioMissaRepository.GetAllAsync(incluirInativos);
        return _mapper.Map<IEnumerable<HorarioMissaDto>>(horarios);
    }

    public async Task<IEnumerable<HorarioMissaDto>> GetByIgrejaAsync(Guid igrejaId, bool incluirInativos = false)
    {
        var horarios = await _horarioMissaRepository.GetByIgrejaAsync(igrejaId, incluirInativos);
        return _mapper.Map<IEnumerable<HorarioMissaDto>>(horarios);
    }

    public async Task<IEnumerable<HorarioMissaDto>> GetByDiaSemanaAsync(DayOfWeek diaSemana, bool incluirInativos = false)
    {
        var horarios = await _horarioMissaRepository.GetByDiaSemanaAsync(diaSemana, incluirInativos);
        return _mapper.Map<IEnumerable<HorarioMissaDto>>(horarios);
    }

    public async Task<HorarioMissaDto> CreateAsync(CreateHorarioMissaDto dto)
    {
        var igreja = await _igrejaRepository.GetByIdAsync(dto.IgrejaId);
        if (igreja == null)
            throw new KeyNotFoundException($"Igreja {dto.IgrejaId} não encontrada");

        var horario = new Domain.Entities.HorarioMissa(
            dto.IgrejaId,
            dto.DiaSemana,
            dto.Horario,
            dto.Celebrante,
            dto.Observacao
        );

        var created = await _horarioMissaRepository.AddAsync(horario);
        return _mapper.Map<HorarioMissaDto>(created);
    }

    public async Task UpdateAsync(Guid id, CreateHorarioMissaDto dto)
    {
        var horario = await _horarioMissaRepository.GetByIdAsync(id);
        if (horario == null)
            throw new KeyNotFoundException($"Horário de Missa {id} não encontrado");

        horario.UpdateInfo(
            dto.DiaSemana,
            dto.Horario,
            dto.Celebrante,
            dto.Observacao
        );

        await _horarioMissaRepository.UpdateAsync(horario);
    }

    public async Task DeleteAsync(Guid id)
    {
        var horario = await _horarioMissaRepository.GetByIdAsync(id);
        if (horario == null)
            throw new KeyNotFoundException($"Horário de Missa {id} não encontrado");

        await _horarioMissaRepository.DeleteAsync(id);
    }
}
