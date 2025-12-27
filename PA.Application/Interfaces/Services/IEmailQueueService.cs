using PA.Application.Services;

namespace PA.Application.Interfaces.Services;

public interface IEmailQueueService
{
    ValueTask QueueEmailAsync(string toEmail, string subject, string message);
    ValueTask<EmailJob?> DequeueEmailAsync(CancellationToken cancellationToken);
}
