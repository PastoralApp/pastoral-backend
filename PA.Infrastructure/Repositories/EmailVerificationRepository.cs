using Microsoft.EntityFrameworkCore;
using PA.Application.Interfaces.Repositories;
using PA.Domain.Entities;
using PA.Infrastructure.Data.Context;

namespace PA.Infrastructure.Repositories;

public class EmailVerificationRepository : IEmailVerificationRepository
{
    private readonly PastoralAppDbContext _context;

    public EmailVerificationRepository(PastoralAppDbContext context)
    {
        _context = context;
    }

    public async Task<EmailVerification> AddAsync(EmailVerification verification)
    {
        await _context.EmailVerifications.AddAsync(verification);
        await _context.SaveChangesAsync();
        return verification;
    }

    public async Task UpdateAsync(EmailVerification verification)
    {
        _context.EmailVerifications.Update(verification);
        await _context.SaveChangesAsync();
    }

    public async Task<EmailVerification?> GetValidCodeAsync(string email, string code, string? purpose = null)
    {
        var query = _context.EmailVerifications
            .Where(ev => ev.Email == email.ToLower() && ev.Code == code);

        if (!string.IsNullOrEmpty(purpose))
            query = query.Where(ev => ev.Purpose == purpose);

        var verification = await query.FirstOrDefaultAsync();
        
        return verification?.IsValid() == true ? verification : null;
    }

    public async Task<EmailVerification?> GetLatestByEmailAsync(string email, string? purpose = null)
    {
        var query = _context.EmailVerifications
            .Where(ev => ev.Email == email.ToLower())
            .OrderByDescending(ev => ev.CreatedAt);

        if (!string.IsNullOrEmpty(purpose))
            query = (IOrderedQueryable<EmailVerification>)query.Where(ev => ev.Purpose == purpose);

        return await query.FirstOrDefaultAsync();
    }

    public async Task DeleteExpiredAsync()
    {
        var expired = await _context.EmailVerifications
            .Where(ev => ev.ExpiresAt < DateTime.UtcNow || ev.IsUsed)
            .ToListAsync();

        _context.EmailVerifications.RemoveRange(expired);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteByEmailAsync(string email)
    {
        var verifications = await _context.EmailVerifications
            .Where(ev => ev.Email == email.ToLower())
            .ToListAsync();

        _context.EmailVerifications.RemoveRange(verifications);
        await _context.SaveChangesAsync();
    }
}
