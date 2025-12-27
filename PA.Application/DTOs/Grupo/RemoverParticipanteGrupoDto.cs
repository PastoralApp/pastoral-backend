using System;

namespace PA.Application.DTOs;

public class RemoverParticipanteGrupoDto
{
    public Guid GrupoId { get; set; }
    public Guid UserId { get; set; }
}
