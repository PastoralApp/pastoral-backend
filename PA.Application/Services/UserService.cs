using AutoMapper;
using PA.Application.DTOs;
using PA.Application.Interfaces.Repositories;
using PA.Application.Interfaces.Services;
using PA.Domain.Entities;
using PA.Domain.Enums;
using PA.Domain.Interfaces;
using PA.Domain.ValueObjects;

namespace PA.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMedalhaRepository _medalhaRepository;
    private readonly ITrofeuRepository _trofeuRepository;
    private readonly IMapper _mapper;

    public UserService(
        IUserRepository userRepository,
        IMedalhaRepository medalhaRepository,
        ITrofeuRepository trofeuRepository,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _medalhaRepository = medalhaRepository;
        _trofeuRepository = trofeuRepository;
        _mapper = mapper;
    }

    public async Task<UserDto?> GetByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto?> GetByEmailAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        return _mapper.Map<UserDto>(user);
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<UserDto>>(users);
    }

    public async Task<IEnumerable<UserDto>> GetByGrupoAsync(Guid grupoId)
    {
        var users = await _userRepository.GetByGrupoIdAsync(grupoId);
        return _mapper.Map<IEnumerable<UserDto>>(users);
    }

    public async Task<UserDto> CreateAsync(CreateUserDto dto)
    {
        if (await _userRepository.EmailExistsAsync(dto.Email))
            throw new InvalidOperationException("Email já cadastrado");

        var email = new Email(dto.Email);
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        var user = new User(dto.Name, email, passwordHash, dto.RoleId);
        var created = await _userRepository.AddAsync(user);

        return _mapper.Map<UserDto>(created);
    }

    public async Task UpdateAsync(Guid id, UpdateUserDto dto)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            throw new KeyNotFoundException("Usuário não encontrado");

        user.UpdateProfile(dto.Name, dto.BirthDate, dto.PhotoUrl);
        await _userRepository.UpdateAsync(user);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _userRepository.DeleteAsync(id);
    }

    public async Task<UserProfileDto> GetProfileAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new KeyNotFoundException("Usuário não encontrado");

        var medalhas = await _medalhaRepository.GetByParticipanteIdAsync(userId);
        var grupos = user.UserGrupos?.Where(ug => ug.IsAtivo).ToList() ?? new List<UserGrupo>();

        var profile = new UserProfileDto
        {
            Id = user.Id,
            Nome = user.Name,
            Email = user.Email.Value,
            Telefone = user.Telefone,
            ImagemPerfilUrl = user.PhotoUrl,
            DataCadastro = user.CreatedAt,
            
            Grupos = grupos.Select(ug => new UserGrupoSimplifiedDto
            {
                GrupoId = ug.GrupoId,
                Nome = ug.Grupo?.Name ?? "",
                Sigla = ug.Grupo?.Sigla,
                LogoUrl = ug.Grupo?.LogoUrl,
                PrimaryColor = ug.Grupo?.Theme?.PrimaryColor,
                SecondaryColor = ug.Grupo?.Theme?.SecondaryColor,
                DataEntrada = ug.DataEntrada,
                IsAtivo = ug.IsAtivo
            }).ToList(),
            
            Medalhas = medalhas.Select(m => new MedalhaSimplifiedDto
            {
                Id = m.Id,
                Tipo = m.Tipo,
                JogoNome = m.Jogo?.Nome,
                ModalidadeNome = m.Modalidade?.Nome,
                GrupoNome = m.Grupo?.Name,
                Ano = m.Ano,
                DataConquista = m.DataConquista
            }).ToList(),
            
            Estatisticas = new UserStatsDto
            {
                TotalMedalhas = medalhas.Count(),
                MedalhasOuro = medalhas.Count(m => m.Tipo == TipoMedalha.Ouro),
                MedalhasPrata = medalhas.Count(m => m.Tipo == TipoMedalha.Prata),
                MedalhasBronze = medalhas.Count(m => m.Tipo == TipoMedalha.Bronze),
                TotalGrupos = grupos.Count,
                GruposAtivos = grupos.Count(g => g.IsAtivo),
                JogosParticipados = medalhas.Select(m => m.JogoId).Distinct().Count()
            }
        };

        return profile;
    }

    public async Task<IEnumerable<UserSelectDto>> GetUsersForSelectAsync(Guid? grupoId = null, Guid? pastoralId = null)
    {
        IEnumerable<User> users;

        if (grupoId.HasValue)
        {
            users = await _userRepository.GetByGrupoIdAsync(grupoId.Value);
        }
        else
        {
            users = await _userRepository.GetAllAsync();
        }

        return users.Select(u => new UserSelectDto
        {
            Id = u.Id,
            Nome = u.Name,
            Email = u.Email.Value,
            ImagemPerfilUrl = u.PhotoUrl,
            IsAtivo = true
        }).ToList();
    }

    public async Task<UserSearchResultDto> SearchUsersAsync(UserSearchDto searchDto)
    {
        var page = searchDto.Page ?? 1;
        var pageSize = searchDto.PageSize ?? 20;

        IEnumerable<User> users;

        if (searchDto.GrupoId.HasValue)
        {
            users = await _userRepository.GetByGrupoIdAsync(searchDto.GrupoId.Value);
        }
        else
        {
            users = await _userRepository.GetAllAsync();
        }

        if (!string.IsNullOrWhiteSpace(searchDto.Nome))
        {
            users = users.Where(u => u.Name.Contains(searchDto.Nome, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(searchDto.Email))
        {
            users = users.Where(u => u.Email.Value.Contains(searchDto.Email, StringComparison.OrdinalIgnoreCase));
        }

        var totalCount = users.Count();
        var pagedUsers = users.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        return new UserSearchResultDto
        {
            Users = pagedUsers.Select(u => new UserSelectDto
            {
                Id = u.Id,
                Nome = u.Name,
                Email = u.Email.Value,
                ImagemPerfilUrl = u.PhotoUrl,
                IsAtivo = true
            }).ToList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }
}
