using PA.Domain.Common;
using PA.Domain.ValueObjects;

namespace PA.Domain.Entities;

/// <summary>
/// Representa um Grupo dentro de uma Pastoral (Agis, Ajax, etc.)
/// </summary>
public class Grupo : AggregateRoot
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Guid PastoralId { get; private set; }
    public ColorTheme Theme { get; private set; }
    public bool IsActive { get; private set; }

    // Navigation
    public Pastoral Pastoral { get; private set; } = null!;
    public ICollection<User> Members { get; private set; }

    private Grupo() 
    { 
        Name = string.Empty;
        Description = string.Empty;
        Theme = new ColorTheme("#000000", "#FFFFFF");
        Members = new List<User>();
    }

    public Grupo(string name, string description, Guid pastoralId, ColorTheme theme)
    {
        Name = name;
        Description = description;
        PastoralId = pastoralId;
        Theme = theme;
        IsActive = true;
        Members = new List<User>();
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
