using AutoMapper;
using PA.Application.DTOs;
using PA.Application.Interfaces.Repositories;
using PA.Application.Interfaces.Services;
using PA.Domain.Entities;
using PA.Domain.Enums;
using PA.Domain.ValueObjects;

namespace PA.Application.Services;

public class PastoralService : IPastoralService
{
    private readonly IPastoralRepository _pastoralRepository;
    private readonly IMapper _mapper;

    public PastoralService(IPastoralRepository pastoralRepository, IMapper mapper)
    {
        _pastoralRepository = pastoralRepository;
        _mapper = mapper;
    }

    public async Task<PastoralDto?> GetByIdAsync(Guid id)
    {
        var pastoral = await _pastoralRepository.GetByIdAsync(id);
        return _mapper.Map<PastoralDto>(pastoral);
    }

    public async Task<IEnumerable<PastoralDto>> GetAllAsync(bool incluirInativas = false)
    {
        var pastorais = await _pastoralRepository.GetAllAsync(incluirInativas);
        return _mapper.Map<IEnumerable<PastoralDto>>(pastorais);
    }

    public async Task<PastoralDto> CreateAsync(CreatePastoralDto dto)
    {
        var pastoral = new Pastoral(
            dto.Name,
            dto.Sigla,
            Enum.Parse<TipoPastoral>(dto.TipoPastoral),
            Enum.Parse<PastoralType>(dto.Type),
            new ColorTheme(dto.PrimaryColor, dto.SecondaryColor),
            dto.Description,
            dto.LogoUrl,
            dto.Icon
        );

        var created = await _pastoralRepository.AddAsync(pastoral);
        return _mapper.Map<PastoralDto>(created);
    }

    public async Task UpdateAsync(Guid id, CreatePastoralDto dto)
    {
        var pastoral = await _pastoralRepository.GetByIdAsync(id);
        if (pastoral == null)
            throw new KeyNotFoundException($"Pastoral {id} não encontrada");

        pastoral.UpdateInfo(dto.Name, dto.Sigla, dto.Description, dto.LogoUrl);
        pastoral.UpdateTheme(new ColorTheme(dto.PrimaryColor, dto.SecondaryColor));
        
        if (!string.IsNullOrEmpty(dto.Icon))
            pastoral.UpdateIcon(dto.Icon);

        await _pastoralRepository.UpdateAsync(pastoral);
    }

    public async Task DeleteAsync(Guid id)
    {
        var pastoral = await _pastoralRepository.GetByIdAsync(id);
        if (pastoral == null)
            throw new KeyNotFoundException($"Pastoral {id} não encontrada");

        await _pastoralRepository.DeleteAsync(id);
    }

    public async Task DesativarAsync(Guid id)
    {
        var pastoral = await _pastoralRepository.GetByIdAsync(id);
        if (pastoral == null)
            throw new KeyNotFoundException($"Pastoral {id} não encontrada");

        pastoral.Deactivate();
        await _pastoralRepository.UpdateAsync(pastoral);
    }

    public async Task AtivarAsync(Guid id)
    {
        var pastoral = await _pastoralRepository.GetByIdAsync(id);
        if (pastoral == null)
            throw new KeyNotFoundException($"Pastoral {id} não encontrada");

        pastoral.Activate();
        await _pastoralRepository.UpdateAsync(pastoral);
    }
}
