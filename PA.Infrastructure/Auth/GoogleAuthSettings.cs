namespace PA.Infrastructure.Auth;

public class GoogleAuthSettings
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string[] RedirectUris { get; set; } = Array.Empty<string>();
}
