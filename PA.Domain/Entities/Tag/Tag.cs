using PA.Domain.Common;

namespace PA.Domain.Entities;

public class Tag : Entity
{
    public string Name { get; private set; }
    public string Color { get; private set; }
    public string Description { get; private set; }
    public ICollection<User> Users { get; private set; }

    private Tag() 
    { 
        Name = string.Empty;
        Color = string.Empty;
        Description = string.Empty;
        Users = new List<User>();
    }

    public Tag(string name, string color, string description)
    {
        Name = name;
        Color = color;
        Description = description;
        Users = new List<User>();
    }

    public void UpdateInfo(string name, string color, string description)
    {
        Name = name;
        Color = color;
        Description = description;
        SetUpdatedAt();
    }
}
