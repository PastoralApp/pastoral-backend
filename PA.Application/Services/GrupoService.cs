using AutoMapper;
using PA.Application.DTOs;
using PA.Application.Interfaces.Repositories;
using PA.Application.Interfaces.Services;
using PA.Domain.Entities;
using PA.Domain.ValueObjects;

namespace PA.Application.Services;

public class GrupoService : IGrupoService
{
    private readonly IGrupoRepository _grupoRepository;
    private readonly IPastoralRepository _pastoralRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserGrupoRepository _userGrupoRepository;
    private readonly IMapper _mapper;

    public GrupoService(
        IGrupoRepository grupoRepository,
        IPastoralRepository pastoralRepository,
        IUserRepository userRepository,
        IUserGrupoRepository userGrupoRepository,
        IMapper mapper)
    {
        _grupoRepository = grupoRepository;
        _pastoralRepository = pastoralRepository;
        _userRepository = userRepository;
        _userGrupoRepository = userGrupoRepository;
        _mapper = mapper;
    }

    public async Task<GrupoDto?> GetByIdAsync(Guid id)
    {
        var grupo = await _grupoRepository.GetByIdAsync(id);
        return _mapper.Map<GrupoDto>(grupo);
    }

    public async Task<IEnumerable<GrupoDto>> GetAllAsync(bool incluirInativos = false)
    {
        var grupos = await _grupoRepository.GetAllAsync(incluirInativos);
        return _mapper.Map<IEnumerable<GrupoDto>>(grupos);
    }

    public async Task<IEnumerable<GrupoDto>> GetByPastoralIdAsync(Guid pastoralId, bool incluirInativos = false)
    {
        var grupos = await _grupoRepository.GetByPastoralIdAsync(pastoralId, incluirInativos);
        return _mapper.Map<IEnumerable<GrupoDto>>(grupos);
    }

    public async Task<GrupoDto> CreateAsync(CreateGrupoDto dto)
    {
        var pastoral = await _pastoralRepository.GetByIdAsync(dto.PastoralId);
        if (pastoral == null)
            throw new KeyNotFoundException($"Pastoral {dto.PastoralId} não encontrada");

        var grupo = new Grupo(
            dto.Name,
            dto.Sigla,
            dto.Description,
            dto.PastoralId,
            new ColorTheme(dto.PrimaryColor, dto.SecondaryColor),
            dto.LogoUrl,
            dto.Icon,
            dto.IgrejaId
        );

        var created = await _grupoRepository.AddAsync(grupo);
        return _mapper.Map<GrupoDto>(created);
    }

    public async Task UpdateAsync(Guid id, CreateGrupoDto dto)
    {
        var grupo = await _grupoRepository.GetByIdAsync(id);
        if (grupo == null)
            throw new KeyNotFoundException($"Grupo {id} não encontrado");

        grupo.UpdateInfo(dto.Name, dto.Sigla, dto.Description, dto.LogoUrl);
        grupo.UpdateTheme(new ColorTheme(dto.PrimaryColor, dto.SecondaryColor));
        grupo.UpdateIgreja(dto.IgrejaId);
        
        if (!string.IsNullOrEmpty(dto.Icon))
            grupo.UpdateIcon(dto.Icon);

        await _grupoRepository.UpdateAsync(grupo);
    }

    public async Task DeleteAsync(Guid id)
    {
        var grupo = await _grupoRepository.GetByIdAsync(id);
        if (grupo == null)
            throw new KeyNotFoundException($"Grupo {id} não encontrado");

        await _grupoRepository.DeleteAsync(id);
    }

    public async Task DesativarAsync(Guid id)
    {
        var grupo = await _grupoRepository.GetByIdAsync(id);
        if (grupo == null)
            throw new KeyNotFoundException($"Grupo {id} não encontrado");

        grupo.Deactivate();
        await _grupoRepository.UpdateAsync(grupo);
    }

    public async Task AtivarAsync(Guid id)
    {
        var grupo = await _grupoRepository.GetByIdAsync(id);
        if (grupo == null)
            throw new KeyNotFoundException($"Grupo {id} não encontrado");

        grupo.Activate();
        await _grupoRepository.UpdateAsync(grupo);
    }

    public async Task AddMemberAsync(Guid grupoId, Guid userId)
    {
        var grupoExists = await _grupoRepository.GetByIdAsync(grupoId);
        if (grupoExists == null)
            throw new KeyNotFoundException($"Grupo {grupoId} não encontrado");

        var userExists = await _userRepository.ExistsAsync(userId);
        if (!userExists)
            throw new KeyNotFoundException($"User {userId} não encontrado");

        var gruposAtivosCount = await _userGrupoRepository.CountActiveGruposByUserAsync(userId);
        if (gruposAtivosCount >= 4)
            throw new InvalidOperationException("Usuário já atingiu o limite de 4 grupos");

        var existingRelation = await _userGrupoRepository.GetByUserAndGrupoAsync(userId, grupoId);
        if (existingRelation != null)
        {
            if (!existingRelation.IsAtivo)
            {
                existingRelation.Ativar();
                await _userGrupoRepository.UpdateAsync(existingRelation);
            }
            return;
        }

        var userGrupo = new UserGrupo(userId, grupoId);
        await _userGrupoRepository.AddAsync(userGrupo);
    }

    public async Task RemoveMemberAsync(Guid grupoId, Guid userId)
    {
        var userGrupo = await _userGrupoRepository.GetByUserAndGrupoAsync(userId, grupoId);
        if (userGrupo == null)
            throw new KeyNotFoundException($"Usuário não está neste grupo");

        userGrupo.Desativar();
        await _userGrupoRepository.UpdateAsync(userGrupo);
    }
}
