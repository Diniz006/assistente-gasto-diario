using System.ComponentModel.DataAnnotations;
using AssistenteGastoDiario.Domain.Enums;

namespace AssistenteGastoDiario.Application.DTOs.Categories;

public sealed record UpdateCategoryRequest(
    [Required, MaxLength(80)] string Name,
    CategoryType Type,
    [MaxLength(20)] string? Color,
    [MaxLength(50)] string? Icon,
    bool IsActive);
