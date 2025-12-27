using AutoMapper;
using PA.Application.DTOs;
using PA.Application.Interfaces.Repositories;
using PA.Application.Interfaces.Services;
using PA.Domain.Entities;

namespace PA.Application.Services;

public class JogoService : IJogoService
{
    private readonly IJogoRepository _jogoRepository;
    private readonly IMapper _mapper;

    public JogoService(IJogoRepository jogoRepository, IMapper mapper)
    {
        _jogoRepository = jogoRepository;
        _mapper = mapper;
    }

    public async Task<JogoDto?> GetByIdAsync(Guid id)
    {
        var jogo = await _jogoRepository.GetByIdAsync(id);
        return jogo == null ? null : _mapper.Map<JogoDto>(jogo);
    }

    public async Task<JogoDto?> GetByIdWithDetailsAsync(Guid id)
    {
        var jogo = await _jogoRepository.GetByIdWithDetailsAsync(id);
        return jogo == null ? null : _mapper.Map<JogoDto>(jogo);
    }

    public async Task<IEnumerable<JogoDto>> GetAllAsync()
    {
        var jogos = await _jogoRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<JogoDto>>(jogos);
    }

    public async Task<JogoDto> CreateAsync(CreateJogoDto dto)
    {
        var jogo = _mapper.Map<Jogo>(dto);
        var created = await _jogoRepository.AddAsync(jogo);
        return _mapper.Map<JogoDto>(created);
    }

    public async Task<JogoDto> UpdateAsync(Guid id, UpdateJogoDto dto)
    {
        var jogo = await _jogoRepository.GetByIdAsync(id);
        if (jogo == null)
            throw new KeyNotFoundException($"Jogo {id} não encontrado");

        _mapper.Map(dto, jogo);
        var updated = await _jogoRepository.UpdateAsync(jogo);
        return _mapper.Map<JogoDto>(updated);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _jogoRepository.DeleteAsync(id);
    }
}
