using PA.Domain.Enums;

namespace PA.Application.DTOs;

public class EventoDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public DateTime? EventEndDate { get; set; }
    public string? Location { get; set; }
    public string? ImageUrl { get; set; }
    public string? BannerUrl { get; set; }
    public int MaxParticipants { get; set; }
    public bool RequireInscription { get; set; }
    public EventoType Type { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public Guid CreatedByUserId { get; set; }
    public string CreatedByName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    public Guid? GrupoId { get; set; }
    public string? GrupoNome { get; set; }
    public Guid? ResponsavelUserId { get; set; }
    public string? ResponsavelUserName { get; set; }
    
    public string? Cor { get; set; }
    
    public string? LinkInscricao { get; set; }
    public decimal? Preco { get; set; }
    public DateTime? DataLimiteInscricao { get; set; }
    
    public int TotalParticipantes { get; set; }
    public bool VagasDisponiveis { get; set; }
    public bool UsuarioParticipando { get; set; }
}

public class CreateEventoDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public DateTime? EventEndDate { get; set; }
    public string? Location { get; set; }
    public string? ImageUrl { get; set; }
    public string? BannerUrl { get; set; }
    public int MaxParticipants { get; set; }
    public bool RequireInscription { get; set; }
    public EventoType Type { get; set; } = EventoType.Comum;
    
    public string? LinkInscricao { get; set; }
    public decimal? Preco { get; set; }
    public DateTime? DataLimiteInscricao { get; set; }
    
    public Guid? GrupoId { get; set; }
    public Guid? ResponsavelUserId { get; set; }
    
    public string? Cor { get; set; }
}

public class UpdateEventoDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public DateTime? EventEndDate { get; set; }
    public string? Location { get; set; }
    public string? ImageUrl { get; set; }
    public string? BannerUrl { get; set; }
    public int MaxParticipants { get; set; }
    public bool RequireInscription { get; set; }
    public EventoType Type { get; set; }
    
    public string? LinkInscricao { get; set; }
    public decimal? Preco { get; set; }
    public DateTime? DataLimiteInscricao { get; set; }
    
    public Guid? GrupoId { get; set; }
    public Guid? ResponsavelUserId { get; set; }
    
    public string? Cor { get; set; }
}

public class EventoParticipanteDto
{
    public Guid Id { get; set; }
    public Guid EventoId { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? UserPhotoUrl { get; set; }
    public bool Confirmado { get; set; }
    public DateTime? DataConfirmacao { get; set; }
}

public class ParticiparEventoDto
{
    public Guid EventoId { get; set; }
}
