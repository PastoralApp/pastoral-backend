using AutoMapper;
using PA.Application.DTOs;
using PA.Application.Interfaces.Repositories;
using PA.Application.Interfaces.Services;
using PA.Domain.Entities;

namespace PA.Application.Services;

public class NotificacaoService : INotificacaoService
{
    private readonly INotificacaoRepository _notificacaoRepository;
    private readonly IUserRepository _userRepository;
    private readonly IGrupoRepository _grupoRepository;
    private readonly IEmailService _emailService;
    private readonly IEmailQueueService _emailQueueService;
    private readonly INotificacaoHubService _notificacaoHubService;
    private readonly IMapper _mapper;

    public NotificacaoService(
        INotificacaoRepository notificacaoRepository,
        IUserRepository userRepository,
        IGrupoRepository grupoRepository,
        IEmailService emailService,
        IEmailQueueService emailQueueService,
        INotificacaoHubService notificacaoHubService,
        IMapper mapper)
    {
        _notificacaoRepository = notificacaoRepository;
        _userRepository = userRepository;
        _grupoRepository = grupoRepository;
        _emailService = emailService;
        _emailQueueService = emailQueueService;
        _notificacaoHubService = notificacaoHubService;
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
        var user = await _userRepository.GetByIdAsync(dto.RemetenteId);
        if (user == null)
            throw new KeyNotFoundException($"User {dto.RemetenteId} não encontrado");

        var notificacao = new Domain.Entities.Notificacao(
            titulo: dto.Titulo,
            mensagem: dto.Mensagem,
            remetenteId: dto.RemetenteId,
            grupoId: dto.GrupoId,
            destinatarioId: dto.DestinatarioId,
            isGeral: dto.IsGeral,
            sendEmail: dto.SendEmail
        );

        var created = await _notificacaoRepository.AddAsync(notificacao);

        await EnviarNotificacaoEmTempoRealAsync(created);

        if (dto.SendEmail)
        {
            await EnviarEmailsNotificacaoAsync(created);
        }

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

    private async Task EnviarEmailsNotificacaoAsync(Notificacao notificacao)
    {
        try
        {
            List<User> destinatarios = new();

            if (notificacao.DestinatarioId.HasValue)
            {
                var destinatario = await _userRepository.GetByIdAsync(notificacao.DestinatarioId.Value);
                if (destinatario != null && destinatario.IsActive && destinatario.IsEmailVerified)
                {
                    destinatarios.Add(destinatario);
                }
            }
            else if (notificacao.IsGeral)
            {
                destinatarios = (await _userRepository.GetAllAsync())
                    .Where(u => u.IsActive && u.IsEmailVerified)
                    .ToList();
            }
            else if (notificacao.GrupoId.HasValue)
            {
                var grupo = await _grupoRepository.GetByIdAsync(notificacao.GrupoId.Value);
                if (grupo != null)
                {
                    destinatarios = grupo.UserGrupos
                        .Where(ug => ug.IsAtivo && ug.User.IsActive && ug.User.IsEmailVerified)
                        .Select(ug => ug.User)
                        .ToList();
                }
            }

            foreach (var user in destinatarios)
            {
                await _emailQueueService.QueueEmailAsync(
                    user.Email.Value,
                    notificacao.Titulo,
                    notificacao.Mensagem
                );
            }

            Console.WriteLine($"{destinatarios.Count} email(s) adicionado(s) à fila");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao adicionar emails à fila: {ex.Message}");
        }
    }

    private async Task EnviarNotificacaoEmTempoRealAsync(Notificacao notificacao)
    {
        try
        {
            var notificacaoDto = _mapper.Map<NotificacaoDto>(notificacao);

            if (notificacao.DestinatarioId.HasValue)
            {
                await _notificacaoHubService.NotificarUsuarioAsync(notificacao.DestinatarioId.Value, notificacaoDto);
            }
            else if (notificacao.IsGeral)
            {
                await _notificacaoHubService.NotificarTodosAsync(notificacaoDto);
            }
            else if (notificacao.GrupoId.HasValue)
            {
                var grupo = await _grupoRepository.GetByIdAsync(notificacao.GrupoId.Value);
                if (grupo != null)
                {
                    var usuariosGrupo = grupo.UserGrupos
                        .Where(ug => ug.IsAtivo && ug.User.IsActive)
                        .Select(ug => ug.User.Id)
                        .ToList();

                    await _notificacaoHubService.NotificarMultiplosUsuariosAsync(usuariosGrupo, notificacaoDto);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao enviar notificação em tempo real: {ex.Message}");
        }
    }
}
