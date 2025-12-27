using AutoMapper;
using PA.Application.DTOs;
using PA.Application.Interfaces.Services;
using PA.Domain.Entities;
using PA.Domain.Interfaces;

namespace PA.Application.Services;

public class TrofeuService : ITrofeuService
{
    private readonly ITrofeuRepository _trofeuRepository;
    private readonly IMapper _mapper;

    public TrofeuService(ITrofeuRepository trofeuRepository, IMapper mapper)
    {
        _trofeuRepository = trofeuRepository;
        _mapper = mapper;
    }

    public async Task<TrofeuDto?> GetByIdAsync(Guid id)
    {
        var trofeu = await _trofeuRepository.GetByIdAsync(id);
        return trofeu == null ? null : _mapper.Map<TrofeuDto>(trofeu);
    }

    public async Task<IEnumerable<TrofeuDto>> GetAllAsync()
    {
        var trofeus = await _trofeuRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<TrofeuDto>>(trofeus);
    }

    public async Task<IEnumerable<TrofeuDto>> GetByJogoIdAsync(Guid jogoId)
    {
        var trofeus = await _trofeuRepository.GetByJogoIdAsync(jogoId);
        return _mapper.Map<IEnumerable<TrofeuDto>>(trofeus);
    }

    public async Task<IEnumerable<TrofeuDto>> GetByGrupoIdAsync(Guid grupoId)
    {
        var trofeus = await _trofeuRepository.GetByGrupoIdAsync(grupoId);
        return _mapper.Map<IEnumerable<TrofeuDto>>(trofeus);
    }

    public async Task<IEnumerable<TrofeuDto>> GetByAnoAsync(int ano)
    {
        var trofeus = await _trofeuRepository.GetByAnoAsync(ano);
        return _mapper.Map<IEnumerable<TrofeuDto>>(trofeus);
    }

    public async Task<TrofeuDto> CreateAsync(CreateTrofeuDto dto)
    {
        var trofeu = new Trofeu
        {
            Nome = dto.Nome,
            Descricao = dto.Descricao,
            JogoId = dto.JogoId,
            GrupoId = dto.GrupoId,
            Ano = dto.Ano,
            DataConquista = dto.DataConquista,
            Categoria = dto.Categoria,
            Posicao = dto.Posicao,
            ImagemUrl = dto.ImagemUrl,
            Observacoes = dto.Observacoes
        };

        var created = await _trofeuRepository.AddAsync(trofeu);
        return _mapper.Map<TrofeuDto>(created);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _trofeuRepository.DeleteAsync(id);
    }
}
