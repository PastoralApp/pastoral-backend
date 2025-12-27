using PA.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace PA.Infrastructure.Services;

public class SmtpEmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly string _fromEmail;
    private readonly string _fromName;
    private readonly string _smtpHost;
    private readonly int _smtpPort;
    private readonly string _smtpUser;
    private readonly string _smtpPassword;
    private readonly bool _enableSsl;

    public SmtpEmailService(IConfiguration configuration)
    {
        _configuration = configuration;
        _fromEmail = configuration["EmailSettings:FromEmail"] ?? "noreply@pastoralapp.com";
        _fromName = configuration["EmailSettings:FromName"] ?? "Pastoral App";
        _smtpHost = configuration["EmailSettings:SmtpHost"] ?? "smtp.gmail.com";
        _smtpPort = int.Parse(configuration["EmailSettings:SmtpPort"] ?? "587");
        _smtpUser = configuration["EmailSettings:SmtpUser"] ?? "";
        _smtpPassword = configuration["EmailSettings:SmtpPassword"] ?? "";
        _enableSsl = bool.Parse(configuration["EmailSettings:EnableSsl"] ?? "true");
    }

    public async Task SendVerificationCodeAsync(string email, string name, string code)
    {
        var subject = "Código de Verificação - Pastoral App";
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                    <h2 style='color: #4A5568;'>Olá, {name}!</h2>
                    <p style='color: #718096; font-size: 16px;'>
                        Obrigado por se registrar no Pastoral App. Use o código abaixo para verificar seu email:
                    </p>
                    <div style='background-color: #EDF2F7; padding: 20px; text-align: center; border-radius: 8px; margin: 20px 0;'>
                        <h1 style='color: #2D3748; font-size: 32px; letter-spacing: 5px; margin: 0;'>{code}</h1>
                    </div>
                    <p style='color: #718096; font-size: 14px;'>
                        Este código expira em 15 minutos.
                    </p>
                    <p style='color: #A0AEC0; font-size: 12px; margin-top: 30px;'>
                        Se você não solicitou este código, ignore este email.
                    </p>
                </div>
            </body>
            </html>
        ";

        await SendEmailAsync(email, subject, body);
    }

    public async Task SendPasswordResetCodeAsync(string email, string name, string code)
    {
        var subject = "Redefinição de Senha - Pastoral App";
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                    <h2 style='color: #4A5568;'>Olá, {name}!</h2>
                    <p style='color: #718096; font-size: 16px;'>
                        Recebemos uma solicitação para redefinir sua senha. Use o código abaixo:
                    </p>
                    <div style='background-color: #EDF2F7; padding: 20px; text-align: center; border-radius: 8px; margin: 20px 0;'>
                        <h1 style='color: #2D3748; font-size: 32px; letter-spacing: 5px; margin: 0;'>{code}</h1>
                    </div>
                    <p style='color: #718096; font-size: 14px;'>
                        Este código expira em 15 minutos.
                    </p>
                    <p style='color: #A0AEC0; font-size: 12px; margin-top: 30px;'>
                        Se você não solicitou esta redefinição, ignore este email.
                    </p>
                </div>
            </body>
            </html>
        ";

        await SendEmailAsync(email, subject, body);
    }

    public async Task SendWelcomeEmailAsync(string email, string name)
    {
        var subject = "Bem-vindo ao Pastoral App!";
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                    <h2 style='color: #4A5568;'>Bem-vindo, {name}!</h2>
                    <p style='color: #718096; font-size: 16px;'>
                        Sua conta foi criada com sucesso no Pastoral App. 
                        Agora você pode acompanhar eventos, grupos e notícias da sua comunidade.
                    </p>
                    <p style='color: #718096; font-size: 16px;'>
                        Que Deus te abençoe!
                    </p>
                    <p style='color: #4A5568; font-size: 14px; margin-top: 30px;'>
                        Equipe Pastoral App
                    </p>
                </div>
            </body>
            </html>
        ";

        await SendEmailAsync(email, subject, body);
    }

    public async Task SendNotificationEmailAsync(string email, string subject, string message)
    {
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                    <h2 style='color: #4A5568;'>{subject}</h2>
                    <div style='color: #718096; font-size: 16px;'>
                        {message}
                    </div>
                    <p style='color: #4A5568; font-size: 14px; margin-top: 30px;'>
                        Equipe Pastoral App
                    </p>
                </div>
            </body>
            </html>
        ";

        await SendEmailAsync(email, subject, body);
    }

    private async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        try
        {
            using var message = new MailMessage();
            message.From = new MailAddress(_fromEmail, _fromName);
            message.To.Add(new MailAddress(toEmail));
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;

            using var smtpClient = new SmtpClient(_smtpHost, _smtpPort);
            smtpClient.Credentials = new NetworkCredential(_smtpUser, _smtpPassword);
            smtpClient.EnableSsl = _enableSsl;

            await smtpClient.SendMailAsync(message);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Erro ao enviar email: {ex.Message}", ex);
        }
    }
}
