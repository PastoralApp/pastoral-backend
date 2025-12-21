using PA.Domain.Common;
using PA.Domain.ValueObjects;

namespace PA.Domain.Entities;

public class User : AggregateRoot
{
    public string Name { get; private set; }
    public Email Email { get; private set; }
    public string PasswordHash { get; private set; }
    public string? PhotoUrl { get; private set; }
    public DateTime? BirthDate { get; private set; }
    public bool IsActive { get; private set; }
    public Guid RoleId { get; private set; }
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

    public void AdicionarAoGrupo(Guid grupoId, Grupo grupo)
    {
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
