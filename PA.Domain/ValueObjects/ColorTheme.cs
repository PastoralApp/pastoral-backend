using System.Text.RegularExpressions;

namespace PA.Domain.ValueObjects;

public class ColorTheme
{
    public string PrimaryColor { get; private set; }
    public string SecondaryColor { get; private set; }

    private ColorTheme() 
    { 
        PrimaryColor = string.Empty;
        SecondaryColor = string.Empty;
    }

    public ColorTheme(string primaryColor, string secondaryColor)
    {
        if (string.IsNullOrWhiteSpace(primaryColor))
            throw new ArgumentException("Cor primária não pode ser vazia", nameof(primaryColor));

        if (string.IsNullOrWhiteSpace(secondaryColor))
            throw new ArgumentException("Cor secundária não pode ser vazia", nameof(secondaryColor));

        if (!IsValidHexColor(primaryColor))
            throw new ArgumentException("Cor primária deve ser um código hexadecimal válido", nameof(primaryColor));

        if (!IsValidHexColor(secondaryColor))
            throw new ArgumentException("Cor secundária deve ser um código hexadecimal válido", nameof(secondaryColor));

        PrimaryColor = primaryColor.ToUpper();
        SecondaryColor = secondaryColor.ToUpper();
    }

    private static bool IsValidHexColor(string color)
    {
        var pattern = @"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$";
        return Regex.IsMatch(color, pattern);
    }

    public override bool Equals(object? obj)
    {
        if (obj is not ColorTheme other)
            return false;

        return PrimaryColor == other.PrimaryColor && SecondaryColor == other.SecondaryColor;
    }

    public override int GetHashCode() => HashCode.Combine(PrimaryColor, SecondaryColor);
}
