using AutoMapper;
using PA.Application.DTOs;
using PA.Application.Interfaces.Services;
using PA.Domain.Entities;
using PA.Domain.Enums;
using PA.Domain.Interfaces;

namespace PA.Application.Services;

public class OlimpiadasService : IOlimpiadasService
{
    private readonly IOlimpiadasRepository _olimpiadasRepository;
    private readonly IGrupoRepository _grupoRepository;
    private readonly IRankingOlimpiadasRepository _rankingRepository;
    private readonly IMapper _mapper;

    public OlimpiadasService(
        IOlimpiadasRepository olimpiadasRepository,
        IGrupoRepository grupoRepository,
        IRankingOlimpiadasRepository rankingRepository,
        IMapper mapper)
    {
        _olimpiadasRepository = olimpiadasRepository;
        _grupoRepository = grupoRepository;
        _rankingRepository = rankingRepository;
        _mapper = mapper;
    }

    public async Task<OlimpiadasDto?> GetByIdAsync(Guid id)
    {
        var olimpiadas = await _olimpiadasRepository.GetByIdAsync(id);
        return olimpiadas == null ? null : _mapper.Map<OlimpiadasDto>(olimpiadas);
    }

    public async Task<OlimpiadasDto?> GetByIdWithDetailsAsync(Guid id)
    {
        var olimpiadas = await _olimpiadasRepository.GetByIdWithModalidadesAsync(id);
        return olimpiadas == null ? null : _mapper.Map<OlimpiadasDto>(olimpiadas);
    }

    public async Task<IEnumerable<OlimpiadasDto>> GetAllAsync()
    {
        var olimpiadas = await _olimpiadasRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<OlimpiadasDto>>(olimpiadas);
    }

    public async Task<IEnumerable<OlimpiadasDto>> GetByPastoralIdAsync(Guid pastoralId)
    {
        var olimpiadas = await _olimpiadasRepository.GetByPastoralIdAsync(pastoralId);
        return _mapper.Map<IEnumerable<OlimpiadasDto>>(olimpiadas);
    }

    public async Task<IEnumerable<OlimpiadasDto>> GetByAnoAsync(int ano)
    {
        var olimpiadas = await _olimpiadasRepository.GetByAnoAsync(ano);
        return _mapper.Map<IEnumerable<OlimpiadasDto>>(olimpiadas);
    }

    public async Task<IEnumerable<OlimpiadasDto>> GetByStatusAsync(StatusJogo status)
    {
        var olimpiadas = await _olimpiadasRepository.GetAllAsync();
        var filtered = olimpiadas.Where(o => o.Status == status);
        return _mapper.Map<IEnumerable<OlimpiadasDto>>(filtered);
    }

    public async Task<OlimpiadasDto> CreateAsync(CreateOlimpiadasDto dto, Guid userId)
    {
        var olimpiadas = _mapper.Map<Olimpiadas>(dto);
        olimpiadas.CriadoPorId = userId;
        olimpiadas.Status = StatusJogo.Planejado;

        var created = await _olimpiadasRepository.AddAsync(olimpiadas);
        return _mapper.Map<OlimpiadasDto>(created);
    }

    public async Task<OlimpiadasDto> UpdateAsync(Guid id, UpdateOlimpiadasDto dto, Guid userId)
    {
        var olimpiadas = await _olimpiadasRepository.GetByIdAsync(id);
        if (olimpiadas == null)
            throw new KeyNotFoundException($"Olimpíada com ID {id} não encontrada.");

        _mapper.Map(dto, olimpiadas);
        olimpiadas.SetUpdatedAt(); 

        await _olimpiadasRepository.UpdateAsync(olimpiadas);
        return _mapper.Map<OlimpiadasDto>(olimpiadas);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _olimpiadasRepository.DeleteAsync(id);
    }

    public async Task<bool> InscreverGrupoAsync(Guid olimpiadasId, Guid grupoId)
    {
        var olimpiadas = await _olimpiadasRepository.GetByIdWithModalidadesAsync(olimpiadasId);
        if (olimpiadas == null)
            return false;

        var grupo = await _grupoRepository.GetByIdAsync(grupoId);
        if (grupo == null)
            return false;

        if (olimpiadas.GruposParticipantes.Any(gj => gj.GrupoId == grupoId))
            return false;

        var grupoJogo = new GrupoJogo
        {
            JogoId = olimpiadasId,
            GrupoId = grupoId,
            DataInscricao = DateTime.UtcNow,
            Confirmado = true
        };

        olimpiadas.GruposParticipantes.Add(grupoJogo);
        await _olimpiadasRepository.UpdateAsync(olimpiadas);

        return true;
    }

    public async Task<bool> RemoverGrupoAsync(Guid olimpiadasId, Guid grupoId)
    {
        var olimpiadas = await _olimpiadasRepository.GetByIdWithModalidadesAsync(olimpiadasId);
        if (olimpiadas == null)
            return false;

        var grupoJogo = olimpiadas.GruposParticipantes.FirstOrDefault(gj => gj.GrupoId == grupoId);
        if (grupoJogo == null)
            return false;

        olimpiadas.GruposParticipantes.Remove(grupoJogo);
        await _olimpiadasRepository.UpdateAsync(olimpiadas);

        return true;
    }

    public async Task<RankingOlimpiadasDto> AtualizarRankingAsync(Guid olimpiadasId)
    {
        await _rankingRepository.RecalcularRankingAsync(olimpiadasId);
        
        var rankings = await _rankingRepository.GetByOlimpiadasIdAsync(olimpiadasId);
        var primeiro = rankings.OrderBy(r => r.Posicao).FirstOrDefault();
        
        return _mapper.Map<RankingOlimpiadasDto>(primeiro);
    }

    public async Task<IEnumerable<RankingOlimpiadasDto>> GetRankingAsync(Guid olimpiadasId)
    {
        var rankings = await _rankingRepository.GetByOlimpiadasIdAsync(olimpiadasId);
        return _mapper.Map<IEnumerable<RankingOlimpiadasDto>>(rankings.OrderBy(r => r.Posicao));
    }
}
