using PA.Domain.Entities;

namespace PA.Application.Interfaces.Repositories;

public interface IEmailVerificationRepository
{
    Task<EmailVerification> AddAsync(EmailVerification verification);
    Task UpdateAsync(EmailVerification verification);
    Task<EmailVerification?> GetValidCodeAsync(string email, string code, string? purpose = null);
    Task<EmailVerification?> GetLatestByEmailAsync(string email, string? purpose = null);
    Task DeleteExpiredAsync();
    Task DeleteByEmailAsync(string email);
}
