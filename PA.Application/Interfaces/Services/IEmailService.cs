using PA.Application.DTOs;

namespace PA.Application.Interfaces.Services;

public interface IEmailService
{
    Task SendVerificationCodeAsync(string email, string name, string code);
    Task SendPasswordResetCodeAsync(string email, string name, string code);
    Task SendWelcomeEmailAsync(string email, string name);
    Task SendNotificationEmailAsync(string email, string subject, string message);
}
