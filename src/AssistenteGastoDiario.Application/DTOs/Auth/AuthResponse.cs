using AssistenteGastoDiario.Application.DTOs.Users;

namespace AssistenteGastoDiario.Application.DTOs.Auth;

public sealed record AuthResponse(
    string AccessToken,
    DateTimeOffset ExpiresAt,
    UserResponse User);
