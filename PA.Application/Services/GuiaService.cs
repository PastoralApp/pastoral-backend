using AutoMapper;
using PA.Application.DTOs;
using PA.Application.Interfaces.Services;
using PA.Domain.Entities;
using PA.Domain.Enums;
using PA.Domain.Interfaces;

namespace PA.Application.Services;

public class GuiaService : IGuiaService
{
    private readonly IGuiaRepository _guiaRepository;
    private readonly IGrupoRepository _grupoRepository;
    private readonly IRankingGuiaRepository _rankingRepository;
    private readonly IMapper _mapper;

    public GuiaService(
        IGuiaRepository guiaRepository,
        IGrupoRepository grupoRepository,
        IRankingGuiaRepository rankingRepository,
        IMapper mapper)
    {
        _guiaRepository = guiaRepository;
        _grupoRepository = grupoRepository;
        _rankingRepository = rankingRepository;
        _mapper = mapper;
    }

    public async Task<GuiaDto?> GetByIdAsync(Guid id)
    {
        var guia = await _guiaRepository.GetByIdAsync(id);
        return guia == null ? null : _mapper.Map<GuiaDto>(guia);
    }

    public async Task<GuiaDto?> GetByIdWithDetailsAsync(Guid id)
    {
        var guia = await _guiaRepository.GetByIdWithProvasAsync(id);
        return guia == null ? null : _mapper.Map<GuiaDto>(guia);
    }

    public async Task<IEnumerable<GuiaDto>> GetAllAsync()
    {
        var guias = await _guiaRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<GuiaDto>>(guias);
    }

    public async Task<IEnumerable<GuiaDto>> GetByPastoralIdAsync(Guid pastoralId)
    {
        var guias = await _guiaRepository.GetByPastoralIdAsync(pastoralId);
        return _mapper.Map<IEnumerable<GuiaDto>>(guias);
    }

    public async Task<IEnumerable<GuiaDto>> GetByAnoAsync(int ano)
    {
        var guias = await _guiaRepository.GetByAnoAsync(ano);
        return _mapper.Map<IEnumerable<GuiaDto>>(guias);
    }

    public async Task<IEnumerable<GuiaDto>> GetByStatusAsync(StatusJogo status)
    {
        var guias = await _guiaRepository.GetAllAsync();
        var filtered = guias.Where(g => g.Status == status);
        return _mapper.Map<IEnumerable<GuiaDto>>(filtered);
    }

    public async Task<GuiaDto> CreateAsync(CreateGuiaDto dto, Guid userId)
    {
        var guia = _mapper.Map<Guia>(dto);
        guia.CriadoPorId = userId;
        guia.Status = StatusJogo.Planejado;

        var created = await _guiaRepository.AddAsync(guia);
        return _mapper.Map<GuiaDto>(created);
    }

    public async Task<GuiaDto> UpdateAsync(Guid id, UpdateGuiaDto dto, Guid userId)
    {
        var guia = await _guiaRepository.GetByIdAsync(id);
        if (guia == null)
            throw new KeyNotFoundException($"Guia com ID {id} não encontrada.");

        _mapper.Map(dto, guia);
        guia.SetUpdatedAt();

        await _guiaRepository.UpdateAsync(guia);
        return _mapper.Map<GuiaDto>(guia);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _guiaRepository.DeleteAsync(id);
    }

    public async Task<bool> InscreverGrupoAsync(Guid guiaId, Guid grupoId)
    {
        var guia = await _guiaRepository.GetByIdWithProvasAsync(guiaId);
        if (guia == null)
            return false;

        var grupo = await _grupoRepository.GetByIdAsync(grupoId);
        if (grupo == null)
            return false;

        if (guia.GruposParticipantes.Any(gj => gj.GrupoId == grupoId))
            return false;

        var grupoJogo = new GrupoJogo
        {
            JogoId = guiaId,
            GrupoId = grupoId,
            DataInscricao = DateTime.UtcNow,
            Confirmado = true
        };

        guia.GruposParticipantes.Add(grupoJogo);
        await _guiaRepository.UpdateAsync(guia);

        return true;
    }

    public async Task<bool> RemoverGrupoAsync(Guid guiaId, Guid grupoId)
    {
        var guia = await _guiaRepository.GetByIdWithProvasAsync(guiaId);
        if (guia == null)
            return false;

        var grupoJogo = guia.GruposParticipantes.FirstOrDefault(gj => gj.GrupoId == grupoId);
        if (grupoJogo == null)
            return false;

        guia.GruposParticipantes.Remove(grupoJogo);
        await _guiaRepository.UpdateAsync(guia);

        return true;
    }

    public async Task<RankingGuiaDto> AtualizarRankingAsync(Guid guiaId)
    {
        await _rankingRepository.RecalcularRankingAsync(guiaId);
        
        var rankings = await _rankingRepository.GetByGuiaIdAsync(guiaId);
        var primeiro = rankings.OrderBy(r => r.Posicao).FirstOrDefault();
        
        return _mapper.Map<RankingGuiaDto>(primeiro);
    }

    public async Task<IEnumerable<RankingGuiaDto>> GetRankingAsync(Guid guiaId)
    {
        var rankings = await _rankingRepository.GetByGuiaIdAsync(guiaId);
        return _mapper.Map<IEnumerable<RankingGuiaDto>>(rankings.OrderBy(r => r.Posicao));
    }
}
