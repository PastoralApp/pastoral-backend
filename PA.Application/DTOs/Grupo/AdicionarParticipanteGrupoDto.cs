using System;

namespace PA.Application.DTOs;

public class AdicionarParticipanteGrupoDto
{
    public Guid GrupoId { get; set; }
    public Guid UserId { get; set; }
    public string? FuncaoNoGrupo { get; set; }
}
