using PA.Domain.Common;

namespace PA.Domain.Entities;

public class EmailVerification : Entity
{
    public string Email { get; private set; }
    public string Code { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public bool IsUsed { get; private set; }
    public string? Purpose { get; private set; } 

    private EmailVerification()
    {
        Email = string.Empty;
        Code = string.Empty;
    }

    public EmailVerification(string email, string code, DateTime expiresAt, string? purpose = "registration")
    {
        Email = email.ToLowerInvariant();
        Code = code;
        ExpiresAt = expiresAt;
        IsUsed = false;
        Purpose = purpose;
    }

    public bool IsValid()
    {
        return !IsUsed && DateTime.UtcNow < ExpiresAt;
    }

    public void MarkAsUsed()
    {
        IsUsed = true;
        SetUpdatedAt();
    }

    public bool VerifyCode(string code)
    {
        return IsValid() && string.Equals(Code, code, StringComparison.OrdinalIgnoreCase);
    }
}
