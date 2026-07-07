using AssistenteGastoDiario.Domain.Enums;

namespace AssistenteGastoDiario.Application.DTOs.Categories;

public sealed record CategoryResponse(
    Guid Id,
    Guid UserId,
    string Name,
    CategoryType Type,
    string? Color,
    string? Icon,
    bool IsDefault,
    bool IsActive,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
