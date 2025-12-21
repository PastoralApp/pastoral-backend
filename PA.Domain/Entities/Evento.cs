using PA.Domain.Common;

namespace PA.Domain.Entities;

public class Evento : AggregateRoot
{
    public string Title { get; private set; }
    public string Description { get; private set; }
    public DateTime EventDate { get; private set; }
    public string? Location { get; private set; }
    public string? ImageUrl { get; private set; }
    public int MaxParticipants { get; private set; }
    public bool RequireInscription { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public User CreatedBy { get; private set; } = null!;

    private Evento() 
    { 
        Title = string.Empty;
        Description = string.Empty;
    }

    public Evento(
        string title, 
        string description, 
        DateTime eventDate, 
        Guid createdByUserId,
        string? location = null,
        string? imageUrl = null,
        int maxParticipants = 0,
        bool requireInscription = false)
    {
        Title = title;
        Description = description;
        EventDate = eventDate;
        Location = location;
        ImageUrl = imageUrl;
        MaxParticipants = maxParticipants;
        RequireInscription = requireInscription;
        CreatedByUserId = createdByUserId;
    }

    public void UpdateInfo(string title, string description, DateTime eventDate, string? location = null)
    {
        Title = title;
        Description = description;
        EventDate = eventDate;
        Location = location;
        SetUpdatedAt();
    }

    public void UpdateImage(string imageUrl)
    {
        ImageUrl = imageUrl;
        SetUpdatedAt();
    }

    public void UpdateCapacity(int maxParticipants)
    {
        MaxParticipants = maxParticipants;
        SetUpdatedAt();
    }
}
