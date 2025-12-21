using PA.Domain.Common;
using PA.Domain.Enums;
using PA.Domain.ValueObjects;

namespace PA.Domain.Entities;

/// <summary>
/// Representa um Grupo dentro de uma Pastoral (Agis, Ajax, etc.)
/// </summary>
public class Grupo : AggregateRoot
{
    public string Name { get; private set; }
    public string Sigla { get; private set; }
    public string Description { get; private set; }
    public Guid PastoralId { get; private set; }
    public ColorTheme Theme { get; private set; }
    public string? LogoUrl { get; private set; }
    public bool IsActive { get; private set; }

    // Navigation
    public Pastoral Pastoral { get; private set; } = null!;
    public ICollection<UserGrupo> UserGrupos { get; private set; }

    private Grupo() 
    { 
        Name = string.Empty;
        Sigla = string.Empty;
        Description = string.Empty;
        Theme = new ColorTheme("#000000", "#FFFFFF");
        UserGrupos = new List<UserGrupo>();
    }

    public Grupo(string name, string sigla, string description, Guid pastoralId, ColorTheme theme, string? logoUrl = null)
    {
        Name = name;
        Sigla = sigla;
        Description = description;
        PastoralId = pastoralId;
        Theme = theme;
        LogoUrl = logoUrl;
        IsActive = true;
        UserGrupos = new List<UserGrupo>();
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

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}
