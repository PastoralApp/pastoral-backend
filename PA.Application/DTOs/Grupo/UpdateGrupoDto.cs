using System;

namespace PA.Application.DTOs;

public class UpdateGrupoDto
{
    public string Name { get; set; } = string.Empty;
    public string Sigla { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid PastoralId { get; set; }
    public Guid? IgrejaId { get; set; }
    public string? LogoUrl { get; set; }
    public string? Icon { get; set; }
    public bool IsActive { get; set; }
}
