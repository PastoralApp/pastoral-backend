using AutoMapper;
using PA.Application.DTOs;
using PA.Application.Interfaces.Repositories;
using PA.Application.Interfaces.Services;

namespace PA.Application.Services;

public class IgrejaService : IIgrejaService
{
    private readonly IIgrejaRepository _igrejaRepository;
    private readonly IMapper _mapper;

    public IgrejaService(IIgrejaRepository igrejaRepository, IMapper mapper)
    {
        _igrejaRepository = igrejaRepository;
        _mapper = mapper;
    }

    public async Task<IgrejaDto?> GetByIdAsync(Guid id)
    {
        var igreja = await _igrejaRepository.GetByIdAsync(id);
        return _mapper.Map<IgrejaDto>(igreja);
    }

    public async Task<IEnumerable<IgrejaDto>> GetAllAsync(bool incluirInativas = false)
    {
        var igrejas = await _igrejaRepository.GetAllAsync(incluirInativas);
        return _mapper.Map<IEnumerable<IgrejaDto>>(igrejas);
    }

    public async Task<IgrejaDto> CreateAsync(CreateIgrejaDto dto)
    {
        var igreja = new Domain.Entities.Igreja(
            dto.Nome,
            dto.Endereco,
            dto.Telefone,
            dto.ImagemUrl
        );

        var created = await _igrejaRepository.AddAsync(igreja);
        return _mapper.Map<IgrejaDto>(created);
    }

    public async Task UpdateAsync(Guid id, CreateIgrejaDto dto)
    {
        var igreja = await _igrejaRepository.GetByIdAsync(id);
        if (igreja == null)
            throw new KeyNotFoundException($"Igreja {id} não encontrada");

        igreja.UpdateInfo(
            dto.Nome,
            dto.Endereco,
            dto.Telefone,
            dto.ImagemUrl
        );

        await _igrejaRepository.UpdateAsync(igreja);
    }

    public async Task DeleteAsync(Guid id)
    {
        var igreja = await _igrejaRepository.GetByIdAsync(id);
        if (igreja == null)
            throw new KeyNotFoundException($"Igreja {id} não encontrada");

        await _igrejaRepository.DeleteAsync(id);
    }

    public async Task DesativarAsync(Guid id)
    {
        var igreja = await _igrejaRepository.GetByIdAsync(id);
        if (igreja == null)
            throw new KeyNotFoundException($"Igreja {id} não encontrada");

        igreja.Deactivate();
        await _igrejaRepository.UpdateAsync(igreja);
    }

    public async Task AtivarAsync(Guid id)
    {
        var igreja = await _igrejaRepository.GetByIdAsync(id);
        if (igreja == null)
            throw new KeyNotFoundException($"Igreja {id} não encontrada");

        igreja.Activate();
        await _igrejaRepository.UpdateAsync(igreja);
    }
}
