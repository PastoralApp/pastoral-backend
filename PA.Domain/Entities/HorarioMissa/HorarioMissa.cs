using PA.Domain.Common;

namespace PA.Domain.Entities;

public class HorarioMissa : Entity
{
    public Guid IgrejaId { get; private set; }
    public DayOfWeek DiaSemana { get; private set; }
    public TimeSpan Horario { get; private set; }
    public string? Celebrante { get; private set; }
    public string? Observacao { get; private set; }
    public bool IsAtivo { get; private set; }
    public Igreja Igreja { get; private set; } = null!;

    private HorarioMissa() { }

    public HorarioMissa(
        Guid igrejaId, 
        DayOfWeek diaSemana, 
        TimeSpan horario, 
        string? celebrante = null,
        string? observacao = null)
    {
        IgrejaId = igrejaId;
        DiaSemana = diaSemana;
        Horario = horario;
        Celebrante = celebrante;
        Observacao = observacao;
        IsAtivo = true;
    }

    public void UpdateInfo(
        DayOfWeek diaSemana, 
        TimeSpan horario, 
        string? celebrante, 
        string? observacao)
    {
        DiaSemana = diaSemana;
        Horario = horario;
        Celebrante = celebrante;
        Observacao = observacao;
    }

    public void Atualizar(
        DayOfWeek diaSemana, 
        TimeSpan horario, 
        string? celebrante, 
        string? observacao)
    {
        UpdateInfo(diaSemana, horario, celebrante, observacao);
    }

    public void Deactivate() => IsAtivo = false;
    public void Activate() => IsAtivo = true;
    public void Desativar() => IsAtivo = false;
    public void Ativar() => IsAtivo = true;
}
