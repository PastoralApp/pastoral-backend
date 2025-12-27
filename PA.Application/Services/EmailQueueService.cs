using System.Threading.Channels;
using PA.Application.Interfaces.Services;

namespace PA.Application.Services;

public record EmailJob(string ToEmail, string Subject, string Message);

public class EmailQueueService : IEmailQueueService
{
    private readonly Channel<EmailJob> _channel;

    public EmailQueueService()
    {
        var options = new BoundedChannelOptions(1000)
        {
            FullMode = BoundedChannelFullMode.Wait
        };
        
        _channel = Channel.CreateBounded<EmailJob>(options);
    }

    public async ValueTask QueueEmailAsync(string toEmail, string subject, string message)
    {
        var emailJob = new EmailJob(toEmail, subject, message);
        await _channel.Writer.WriteAsync(emailJob);
    }

    public async ValueTask<EmailJob?> DequeueEmailAsync(CancellationToken cancellationToken)
    {
        var emailJob = await _channel.Reader.ReadAsync(cancellationToken);
        return emailJob;
    }
}
