using PA.Domain.Common;

namespace PA.Domain.Entities;

public class Igreja : Entity
{
    public string Nome { get; private set; }
    public string? Endereco { get; private set; }
    public string? Telefone { get; private set; }
    public string? ImagemUrl { get; private set; }
    public bool IsAtiva { get; private set; }
    public ICollection<HorarioMissa> HorariosMissas { get; private set; } = new List<HorarioMissa>();

    private Igreja() { Nome = string.Empty; }

    public Igreja(string nome, string? endereco = null, string? telefone = null, string? imagemUrl = null)
    {
        Nome = nome;
        Endereco = endereco;
        Telefone = telefone;
        ImagemUrl = imagemUrl;
        IsAtiva = true;
    }

    public void UpdateInfo(string nome, string? endereco, string? telefone, string? imagemUrl)
    {
        Nome = nome;
        Endereco = endereco;
        Telefone = telefone;
        ImagemUrl = imagemUrl;
    }

    public void Atualizar(string nome, string? endereco, string? telefone, string? imagemUrl)
    {
        UpdateInfo(nome, endereco, telefone, imagemUrl);
    }

    public void Deactivate() => IsAtiva = false;
    public void Activate() => IsAtiva = true;
    public void Desativar() => IsAtiva = false;
    public void Ativar() => IsAtiva = true;
}
