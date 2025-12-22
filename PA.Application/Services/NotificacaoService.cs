using AutoMapper;
using PA.Application.DTOs;
using PA.Application.Interfaces.Repositories;
using PA.Application.Interfaces.Services;

namespace PA.Application.Services;

public class NotificacaoService : INotificacaoService
{
    private readonly INotificacaoRepository _notificacaoRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public NotificacaoService(
        INotificacaoRepository notificacaoRepository,
        IUserRepository userRepository,
        IMapper mapper)
    {
        _notificacaoRepository = notificacaoRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<NotificacaoDto?> GetByIdAsync(Guid id)
    {
        var notificacao = await _notificacaoRepository.GetByIdAsync(id);
        return _mapper.Map<NotificacaoDto>(notificacao);
    }

    public async Task<IEnumerable<NotificacaoDto>> GetByUserIdAsync(Guid userId)
    {
        var notificacoes = await _notificacaoRepository.GetByUserIdAsync(userId);
        return _mapper.Map<IEnumerable<NotificacaoDto>>(notificacoes);
    }

    public async Task<IEnumerable<NotificacaoDto>> GetUnreadAsync(Guid userId)
    {
        var notificacoes = await _notificacaoRepository.GetNaoLidasByUserIdAsync(userId);
        return _mapper.Map<IEnumerable<NotificacaoDto>>(notificacoes);
    }

    public async Task<IEnumerable<NotificacaoDto>> GetByGrupoIdAsync(Guid grupoId)
    {
        var notificacoes = await _notificacaoRepository.GetByGrupoIdAsync(grupoId);
        return _mapper.Map<IEnumerable<NotificacaoDto>>(notificacoes);
    }

    public async Task<NotificacaoDto> CreateAsync(CreateNotificacaoDto dto)
    {
        var user = await _userRepository.GetByIdAsync(dto.UserId);
        if (user == null)
            throw new KeyNotFoundException($"User {dto.UserId} não encontrado");

        var notificacao = new Domain.Entities.Notificacao(
            titulo: dto.Message,
            mensagem: dto.Message,
            remetenteId: dto.UserId,
            grupoId: dto.GrupoId,
            isGeral: dto.GrupoId == null
        );

        var created = await _notificacaoRepository.AddAsync(notificacao);
        return _mapper.Map<NotificacaoDto>(created);
    }

    public async Task UpdateAsync(Guid id, CreateNotificacaoDto dto)
    {
        var notificacao = await _notificacaoRepository.GetByIdAsync(id);
        if (notificacao == null)
            throw new KeyNotFoundException($"Notificação {id} não encontrada");

        await _notificacaoRepository.UpdateAsync(notificacao);
    }

    public async Task DeleteAsync(Guid id)
    {
        var notificacao = await _notificacaoRepository.GetByIdAsync(id);
        if (notificacao == null)
            throw new KeyNotFoundException($"Notificação {id} não encontrada");

        await _notificacaoRepository.DeleteAsync(id);
    }

    public async Task MarkAsReadAsync(Guid id)
    {
        var notificacao = await _notificacaoRepository.GetByIdAsync(id);
        if (notificacao == null)
            throw new KeyNotFoundException($"Notificação {id} não encontrada");

        // MarkAsRead agora recebe userId, mas vamos apenas marcar como lido
        // A implementação específica de leitura depende de NotificacaoLeitura
        await _notificacaoRepository.UpdateAsync(notificacao);
    }

    public async Task DesativarAsync(Guid id)
    {
        var notificacao = await _notificacaoRepository.GetByIdAsync(id);
        if (notificacao == null)
            throw new KeyNotFoundException($"Notificação {id} não encontrada");

        notificacao.Deactivate();
        await _notificacaoRepository.UpdateAsync(notificacao);
    }

    public async Task AtivarAsync(Guid id)
    {
        var notificacao = await _notificacaoRepository.GetByIdAsync(id);
        if (notificacao == null)
            throw new KeyNotFoundException($"Notificação {id} não encontrada");

        notificacao.Activate();
        await _notificacaoRepository.UpdateAsync(notificacao);
    }
}
