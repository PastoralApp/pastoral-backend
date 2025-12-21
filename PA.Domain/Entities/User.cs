using PA.Domain.Common;
using PA.Domain.ValueObjects;

namespace PA.Domain.Entities;

/// <summary>
/// Representa um Usuário do sistema
/// </summary>
public class User : AggregateRoot
{
    public string Name { get; private set; }
    public Email Email { get; private set; }
    public string PasswordHash { get; private set; }
    public string? PhotoUrl { get; private set; }
    public DateTime? BirthDate { get; private set; }
    public bool IsActive { get; private set; }

    // Relacionamentos
    public Guid RoleId { get; private set; }

    // Navigation
    public Role Role { get; private set; } = null!;
    public ICollection<UserGrupo> UserGrupos { get; private set; }
    public ICollection<Tag> Tags { get; private set; }
    public ICollection<Post> Posts { get; private set; }
    public ICollection<Evento> EventosCreated { get; private set; }

    private User() 
    { 
        Name = string.Empty;
        Email = new Email("default@email.com");
        PasswordHash = string.Empty;
        UserGrupos = new List<UserGrupo>();
        Tags = new List<Tag>();
        Posts = new List<Post>();
        EventosCreated = new List<Evento>();
    }

    public User(string name, Email email, string passwordHash, Guid roleId)
    {
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        RoleId = roleId;
        IsActive = true;
        UserGrupos = new List<UserGrupo>();
        Tags = new List<Tag>();
        Posts = new List<Post>();
        EventosCreated = new List<Evento>();
    }

    public void UpdateProfile(string name, DateTime? birthDate, string? photoUrl)
    {
        Name = name;
        BirthDate = birthDate;
        PhotoUrl = photoUrl;
        SetUpdatedAt();
    }

    public void UpdateEmail(Email email)
    {
        Email = email;
        SetUpdatedAt();
    }

    public void UpdatePassword(string passwordHash)
    {
        PasswordHash = passwordHash;
        SetUpdatedAt();
    }

    public void UpdateRole(Guid roleId)
    {
        RoleId = roleId;
        SetUpdatedAt();
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;

    public void AddTag(Tag tag)
    {
        if (!Tags.Contains(tag))
            Tags.Add(tag);
    }

    public void RemoveTag(Tag tag)
    {
        Tags.Remove(tag);
    }

    /// <summary>
    /// Adiciona usuário a um grupo validando limites (max 1 PA_Cima, 1 PA_Baixo, 1 PJ_Cima, 1 PJ_Baixo)
    /// </summary>
    public void AdicionarAoGrupo(Guid grupoId, Grupo grupo)
    {
        // Validação será feita no service
        var userGrupo = new UserGrupo(Id, grupoId);
        UserGrupos.Add(userGrupo);
        SetUpdatedAt();
    }

    public void RemoverDoGrupo(Guid grupoId)
    {
        var userGrupo = UserGrupos.FirstOrDefault(ug => ug.GrupoId == grupoId);
        if (userGrupo != null)
        {
            userGrupo.Desativar();
            SetUpdatedAt();
        }
    }
}
