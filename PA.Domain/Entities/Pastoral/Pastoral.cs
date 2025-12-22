using PA.Domain.Common;
using PA.Domain.Enums;
using PA.Domain.ValueObjects;

namespace PA.Domain.Entities;

public class Pastoral : AggregateRoot
{
    public string Name { get; private set; }
    public string Sigla { get; private set; }

    public string? Icon { get; private set; }
    public TipoPastoral TipoPastoral { get; private set; }
    public PastoralType Type { get; private set; }
    public ColorTheme Theme { get; private set; }
    public string Description { get; private set; }
    public string? LogoUrl { get; private set; }
    public bool IsActive { get; private set; }
    public ICollection<Grupo> Grupos { get; private set; }

    private Pastoral() 
    { 
        Name = string.Empty;
        Sigla = string.Empty;
        Description = string.Empty;
        Theme = new ColorTheme("#000000", "#FFFFFF");
        Grupos = new List<Grupo>();
    }

    public Pastoral(
        string name, 
        string sigla,
        TipoPastoral tipoPastoral,
        PastoralType type, 
        ColorTheme theme, 
        string description,
        string? logoUrl = null,
        string? icon = null)
    {
        Name = name;
        Sigla = sigla;
        TipoPastoral = tipoPastoral;
        Type = type;
        Theme = theme;
        Description = description;
        LogoUrl = logoUrl;
        Icon = icon;
        IsActive = true;
        Grupos = new List<Grupo>();
    }

    public void UpdateInfo(string name, string sigla, string description, string? logoUrl)
    {
        Name = name;
        Sigla = sigla;
        Description = description;
        LogoUrl = logoUrl;
        SetUpdatedAt();
    }

    public void UpdateTheme(ColorTheme theme)
    {
        Theme = theme;
        SetUpdatedAt();
    }

    public void UpdateIcon(string? icon)
    {
        Icon = icon;
        SetUpdatedAt();
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}
