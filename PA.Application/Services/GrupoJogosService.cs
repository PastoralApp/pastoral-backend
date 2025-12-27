using AutoMapper;
using PA.Application.DTOs;
using PA.Application.Interfaces.Services;
using PA.Domain.Entities;
using PA.Domain.Interfaces;

namespace PA.Application.Services;

public class GrupoJogosService : IGrupoService
{
    private readonly IGrupoRepository _grupoRepository;
    private readonly IMedalhaRepository _medalhaRepository;
    private readonly ITrofeuRepository _trofeuRepository;
    private readonly IMapper _mapper;

    public GrupoJogosService(
        IGrupoRepository grupoRepository,
        IMedalhaRepository medalhaRepository,
        ITrofeuRepository trofeuRepository,
        IMapper mapper)
    {
        _grupoRepository = grupoRepository;
        _medalhaRepository = medalhaRepository;
        _trofeuRepository = trofeuRepository;
        _mapper = mapper;
    }

    public async Task<GrupoDto?> GetByIdAsync(Guid id)
    {
        var grupo = await _grupoRepository.GetByIdAsync(id);
        return grupo == null ? null : _mapper.Map<GrupoDto>(grupo);
    }

    public async Task<GrupoDto?> GetByIdWithParticipantesAsync(Guid id)
    {
        var grupo = await _grupoRepository.GetByIdWithParticipantesAsync(id);
        return grupo == null ? null : _mapper.Map<GrupoDto>(grupo);
    }

    public async Task<GrupoDto?> GetByIdWithConquistasAsync(Guid id)
    {
        var grupo = await _grupoRepository.GetByIdWithConquistasAsync(id);
        return grupo == null ? null : _mapper.Map<GrupoDto>(grupo);
    }

    public async Task<IEnumerable<GrupoDto>> GetAllAsync()
    {
        var grupos = await _grupoRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<GrupoDto>>(grupos);
    }

    public async Task<IEnumerable<GrupoDto>> GetByPastoralIdAsync(Guid pastoralId)
    {
        var grupos = await _grupoRepository.GetByPastoralIdAsync(pastoralId);
        return _mapper.Map<IEnumerable<GrupoDto>>(grupos);
    }

    public async Task<IEnumerable<GrupoDto>> GetAtivosByPastoralIdAsync(Guid pastoralId)
    {
        var grupos = await _grupoRepository.GetAtivosByPastoralIdAsync(pastoralId);
        return _mapper.Map<IEnumerable<GrupoDto>>(grupos);
    }

    public async Task<GrupoDto> CreateAsync(CreateGrupoDto dto)
    {
        var grupo = _mapper.Map<Grupo>(dto);

        var created = await _grupoRepository.AddAsync(grupo);
        return _mapper.Map<GrupoDto>(created);
    }

    public async Task<GrupoDto> UpdateAsync(Guid id, UpdateGrupoDto dto)
    {
        var grupo = await _grupoRepository.GetByIdAsync(id);
        if (grupo == null)
            throw new KeyNotFoundException($"Grupo com ID {id} não encontrado.");

        _mapper.Map(dto, grupo);
        grupo.SetUpdatedAt(); 

        await _grupoRepository.UpdateAsync(grupo);
        return _mapper.Map<GrupoDto>(grupo);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _grupoRepository.DeleteAsync(id);
    }

    public async Task<bool> AdicionarParticipanteAsync(AdicionarParticipanteGrupoDto dto)
    {
        var grupo = await _grupoRepository.GetByIdWithParticipantesAsync(dto.GrupoId);
        if (grupo == null)
            return false;

        if (grupo.UserGrupos.Any(p => p.UserId == dto.UserId && p.IsAtivo))
            return false;

        var userGrupo = new UserGrupo(dto.UserId, dto.GrupoId);

        await _grupoRepository.UpdateAsync(grupo);

        return true;
    }

    public async Task<bool> RemoverParticipanteAsync(RemoverParticipanteGrupoDto dto)
    {
        var grupo = await _grupoRepository.GetByIdWithParticipantesAsync(dto.GrupoId);
        if (grupo == null)
            return false;

        var userGrupo = grupo.UserGrupos.FirstOrDefault(p => p.UserId == dto.UserId && p.IsAtivo);
        if (userGrupo == null)
            return false;

        userGrupo.Desativar();

        await _grupoRepository.UpdateAsync(grupo);

        return true;
    }

    public async Task<IEnumerable<MedalhaDto>> GetMedalhasAsync(Guid grupoId)
    {
        var medalhas = await _medalhaRepository.GetByGrupoIdAsync(grupoId);
        return _mapper.Map<IEnumerable<MedalhaDto>>(medalhas);
    }

    public async Task<IEnumerable<TrofeuDto>> GetTrofeusAsync(Guid grupoId)
    {
        var trofeus = await _trofeuRepository.GetByGrupoIdAsync(grupoId);
        return _mapper.Map<IEnumerable<TrofeuDto>>(trofeus);
    }
}
