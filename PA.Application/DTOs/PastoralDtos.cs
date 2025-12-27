namespace PA.Application.DTOs;

public class IgrejaDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Endereco { get; set; }
    public string? Telefone { get; set; }
    public string? ImagemUrl { get; set; }
    public bool IsAtiva { get; set; }
    public List<HorarioMissaDto> HorariosMissas { get; set; } = new();
}

public class HorarioMissaDto
{
    public Guid Id { get; set; }
    public Guid IgrejaId { get; set; }
    public string? IgrejaNome { get; set; }
    public DayOfWeek DiaSemana { get; set; }
    public string DiaSemanaTexto { get; set; } = string.Empty;
    public TimeSpan Horario { get; set; }
    public string HorarioTexto { get; set; } = string.Empty;
    public string? Celebrante { get; set; }
    public string? Observacao { get; set; }
    public bool IsAtivo { get; set; }
}

public class GrupoDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Sigla { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid PastoralId { get; set; }
    public string? PastoralName { get; set; }
    public string? PastoralSigla { get; set; }
    public Guid? IgrejaId { get; set; }
    public string? IgrejaNome { get; set; }
    public string PrimaryColor { get; set; } = string.Empty;
    public string SecondaryColor { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string? Icon { get; set; }
    public bool IsActive { get; set; }
    public int MembersCount { get; set; }
}

public class PastoralDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Sigla { get; set; } = string.Empty;
    public string TipoPastoral { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string PrimaryColor { get; set; } = string.Empty;
    public string SecondaryColor { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string? Icon { get; set; }
    public bool IsActive { get; set; }
    public List<GrupoDto> Grupos { get; set; } = new();
}

public class CreateIgrejaDto
{
    public string Nome { get; set; } = string.Empty;
    public string? Endereco { get; set; }
    public string? Telefone { get; set; }
    public string? ImagemUrl { get; set; }
}

public class CreateHorarioMissaDto
{
    public Guid IgrejaId { get; set; }
    public DayOfWeek DiaSemana { get; set; }
    public TimeSpan Horario { get; set; }
    public string? Celebrante { get; set; }
    public string? Observacao { get; set; }
}

public class UpdateUserProfileDto
{
    public string Name { get; set; } = string.Empty;
    public DateTime? BirthDate { get; set; }
    public string? PhotoUrl { get; set; }
}

public class UpdateUserDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime? BirthDate { get; set; }
    public string? PhotoUrl { get; set; }
    public Guid RoleId { get; set; }
    public bool IsActive { get; set; }
}

public class UpdateUserRoleDto
{
    public Guid RoleId { get; set; }
}

public class AddUserToGrupoDto
{
    public Guid UserId { get; set; }
    public Guid GrupoId { get; set; }
}

public record CreatePastoralDto(
    string Name,
    string Sigla,
    string TipoPastoral,
    string Type,
    string Description,
    string PrimaryColor,
    string SecondaryColor,
    string? LogoUrl,
    string? Icon
);

public record CreateGrupoDto(
    string Name,
    string Sigla,
    string Description,
    Guid PastoralId,
    Guid? IgrejaId,
    string PrimaryColor,
    string SecondaryColor,
    string? LogoUrl,
    string? Icon
);
