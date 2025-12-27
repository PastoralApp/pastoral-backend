using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PA.Application.Interfaces.Services;
using PA.Application.Services;

namespace PA.Infrastructure.Services;

public class EmailQueueBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EmailQueueBackgroundService> _logger;
    private readonly IEmailQueueService _emailQueueService;

    public EmailQueueBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<EmailQueueBackgroundService> logger,
        IEmailQueueService emailQueueService)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _emailQueueService = emailQueueService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Email Queue Background Service está iniciando");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var emailJob = await _emailQueueService.DequeueEmailAsync(stoppingToken);
                
                if (emailJob != null)
                {
                    await ProcessEmailAsync(emailJob, stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar email da fila");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }

        _logger.LogInformation("Email Queue Background Service está parando");
    }

    private async Task ProcessEmailAsync(EmailJob emailJob, CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

        try
        {
            _logger.LogInformation("Enviando email para {Email}", emailJob.ToEmail);
            
            await emailService.SendNotificationEmailAsync(
                emailJob.ToEmail,
                emailJob.Subject,
                emailJob.Message
            );
            
            _logger.LogInformation("Email enviado com sucesso para {Email}", emailJob.ToEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar email para {Email}", emailJob.ToEmail);
            
         
        }
    }
}
