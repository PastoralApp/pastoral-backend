using PA.Domain.Common;
using PA.Domain.Enums;
using PA.Domain.ValueObjects;

namespace PA.Domain.Entities;

public class Grupo : AggregateRoot
{
    public string Name { get; private set; }
    public string Sigla { get; private set; }
    public string Description { get; private set; }
    public Guid PastoralId { get; private set; }
    public Guid? IgrejaId { get; private set; }
    public ColorTheme Theme { get; private set; }
    public string? LogoUrl { get; private set; }
    public string? Icon { get; private set; }
    public bool IsActive { get; private set; }
    public Pastoral Pastoral { get; private set; } = null!;
    public Igreja? Igreja { get; private set; }
    public ICollection<UserGrupo> UserGrupos { get; private set; }

    private Grupo() 
    { 
        Name = string.Empty;
        Sigla = string.Empty;
        Description = string.Empty;
        Theme = new ColorTheme("#000000", "#FFFFFF");
        UserGrupos = new List<UserGrupo>();
    }

    public Grupo(string name, string sigla, string description, Guid pastoralId, ColorTheme theme, string? logoUrl = null, string? icon = null, Guid? igrejaId = null)
    {
        Name = name;
        Sigla = sigla;
        Description = description;
        PastoralId = pastoralId;
        IgrejaId = igrejaId;
        Theme = theme;
        LogoUrl = logoUrl;
        Icon = icon;
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

    public void UpdateIcon(string? icon)
    {
        Icon = icon;
        SetUpdatedAt();
    }

    public void UpdateIgreja(Guid? igrejaId)
    {
        IgrejaId = igrejaId;
        SetUpdatedAt();
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}
