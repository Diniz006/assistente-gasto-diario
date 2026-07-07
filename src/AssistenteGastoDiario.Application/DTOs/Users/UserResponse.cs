namespace AssistenteGastoDiario.Application.DTOs.Users;

public sealed record UserResponse(
    Guid Id,
    string Name,
    string Email,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
