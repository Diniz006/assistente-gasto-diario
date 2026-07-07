namespace AssistenteGastoDiario.Infrastructure.Auth;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "AssistenteGastoDiario";
    public string Audience { get; set; } = "AssistenteGastoDiario.Api";
    public string SigningKey { get; set; } = "dev-only-change-this-signing-key-with-at-least-32-chars";
    public int ExpiresInMinutes { get; set; } = 120;
}
