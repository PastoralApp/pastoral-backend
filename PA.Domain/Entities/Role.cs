using PA.Domain.Common;
using PA.Domain.Enums;

namespace PA.Domain.Entities;

/// <summary>
/// Representa uma Role (permissão) no sistema
/// </summary>
public class Role : Entity
{
    public string Name { get; private set; }
    public RoleType Type { get; private set; }
    public string Description { get; private set; }

    // Navigation
    public ICollection<User> Users { get; private set; }

    private Role() 
    { 
        Name = string.Empty;
        Description = string.Empty;
        Users = new List<User>();
    }

    public Role(string name, RoleType type, string description)
    {
        Name = name;
        Type = type;
        Description = description;
        Users = new List<User>();
    }

    public void UpdateInfo(string name, string description)
    {
        Name = name;
        Description = description;
        SetUpdatedAt();
    }
}
