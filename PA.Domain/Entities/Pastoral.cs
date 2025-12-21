using PA.Domain.Common;
using PA.Domain.Enums;
using PA.Domain.ValueObjects;

namespace PA.Domain.Entities;

/// <summary>
/// Representa uma Pastoral (PA ou PJ)
/// </summary>
public class Pastoral : AggregateRoot
{
    public string Name { get; private set; }
    public PastoralType Type { get; private set; }
    public ColorTheme Theme { get; private set; }
    public string Description { get; private set; }
    public bool IsActive { get; private set; }

    // Navigation
    public ICollection<Grupo> Grupos { get; private set; }

    private Pastoral() 
    { 
        Name = string.Empty;
        Description = string.Empty;
        Theme = new ColorTheme("#000000", "#FFFFFF");
        Grupos = new List<Grupo>();
    }

    public Pastoral(string name, PastoralType type, ColorTheme theme, string description)
    {
        Name = name;
        Type = type;
        Theme = theme;
        Description = description;
        IsActive = true;
        Grupos = new List<Grupo>();
    }

    public void UpdateInfo(string name, string description)
    {
        Name = name;
        Description = description;
        SetUpdatedAt();
    }

    public void UpdateTheme(ColorTheme theme)
    {
        Theme = theme;
        SetUpdatedAt();
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}
