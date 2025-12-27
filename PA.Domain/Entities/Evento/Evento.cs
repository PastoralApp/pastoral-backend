using PA.Domain.Common;
using PA.Domain.Enums;

namespace PA.Domain.Entities;

public class Evento : AggregateRoot
{
    public string Title { get; private set; }
    public string Description { get; private set; }
    public DateTime EventDate { get; private set; }
    public DateTime? EventEndDate { get; private set; }
    public string? Location { get; private set; }
    public string? ImageUrl { get; private set; }
    public string? BannerUrl { get; private set; }
    public int MaxParticipants { get; private set; }
    public bool RequireInscription { get; private set; }
    public EventoType Type { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public User CreatedBy { get; private set; } = null!;
    
    public Guid? GrupoId { get; private set; }
    public Grupo? Grupo { get; private set; }
    public Guid? ResponsavelUserId { get; private set; }
    public User? ResponsavelUser { get; private set; }
    
    public string? Cor { get; private set; }
    
    public string? LinkInscricao { get; private set; }
    public decimal? Preco { get; private set; }
    public DateTime? DataLimiteInscricao { get; private set; }

    
    private readonly List<EventoParticipante> _participantes = new();
    public IReadOnlyCollection<EventoParticipante> Participantes => _participantes.AsReadOnly();

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
        EventoType type = EventoType.Comum,
        string? location = null,
        string? imageUrl = null,
        string? bannerUrl = null,
        int maxParticipants = 0,
        bool requireInscription = false,
        DateTime? eventEndDate = null,
        string? linkInscricao = null,
        decimal? preco = null,
        DateTime? dataLimiteInscricao = null,
        Guid? grupoId = null,
        Guid? responsavelUserId = null,
        string? cor = null)
    {
        Title = title;
        Description = description;
        EventDate = eventDate;
        EventEndDate = eventEndDate;
        Location = location;
        ImageUrl = imageUrl;
        BannerUrl = bannerUrl;
        MaxParticipants = maxParticipants;
        RequireInscription = requireInscription;
        Type = type;
        CreatedByUserId = createdByUserId;
        LinkInscricao = linkInscricao;
        Preco = preco;
        DataLimiteInscricao = dataLimiteInscricao;
        GrupoId = grupoId;
        ResponsavelUserId = responsavelUserId;
        Cor = cor;
    }

    public void UpdateInfo(string title, string description, DateTime eventDate, string? location = null, DateTime? eventEndDate = null)
    {
        Title = title;
        Description = description;
        EventDate = eventDate;
        EventEndDate = eventEndDate;
        Location = location;
        SetUpdatedAt();
    }

    public void UpdateImage(string imageUrl)
    {
        ImageUrl = imageUrl;
        SetUpdatedAt();
    }

    public void UpdateBanner(string bannerUrl)
    {
        BannerUrl = bannerUrl;
        SetUpdatedAt();
    }

    public void UpdateCapacity(int maxParticipants)
    {
        MaxParticipants = maxParticipants;
        SetUpdatedAt();
    }

    public void UpdateType(EventoType type)
    {
        Type = type;
        SetUpdatedAt();
    }

    public void UpdateInscricaoInfo(string? linkInscricao, decimal? preco, DateTime? dataLimiteInscricao)
    {
        LinkInscricao = linkInscricao;
        Preco = preco;
        DataLimiteInscricao = dataLimiteInscricao;
        SetUpdatedAt();
    }

    public void UpdateGrupo(Guid? grupoId)
    {
        GrupoId = grupoId;
        SetUpdatedAt();
    }

    public void UpdateResponsavel(Guid? responsavelUserId)
    {
        ResponsavelUserId = responsavelUserId;
        SetUpdatedAt();
    }

    public void UpdateCor(string? cor)
    {
        Cor = cor;
        SetUpdatedAt();
    }

    public void AdicionarParticipante(EventoParticipante participante)
    {
        _participantes.Add(participante);
        SetUpdatedAt();
    }

    public void RemoverParticipante(EventoParticipante participante)
    {
        _participantes.Remove(participante);
        SetUpdatedAt();
    }

    public int TotalParticipantes => _participantes.Count(p => p.Confirmado);
    public bool VagasDisponiveis => MaxParticipants == 0 || TotalParticipantes < MaxParticipants;
}
