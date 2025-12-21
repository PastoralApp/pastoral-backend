using System.Text.RegularExpressions;

namespace PA.Domain.ValueObjects;

/// <summary>
/// Value Object para representar Email
/// </summary>
public class Email
{
    public string Value { get; private set; }

    private Email() { Value = string.Empty; }

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email não pode ser vazio", nameof(value));

        if (!IsValidEmail(value))
            throw new ArgumentException("Email inválido", nameof(value));

        Value = value.ToLower().Trim();
    }

    private static bool IsValidEmail(string email)
    {
        var pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, pattern);
    }

    public override string ToString() => Value;

    public override bool Equals(object? obj)
    {
        if (obj is not Email other)
            return false;

        return Value == other.Value;
    }

    public override int GetHashCode() => Value.GetHashCode();

    public static implicit operator string(Email email) => email.Value;
    public static explicit operator Email(string value) => new(value);
}
