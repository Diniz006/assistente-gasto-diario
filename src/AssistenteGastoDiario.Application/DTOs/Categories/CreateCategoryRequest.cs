using System.ComponentModel.DataAnnotations;
using AssistenteGastoDiario.Domain.Enums;

namespace AssistenteGastoDiario.Application.DTOs.Categories;

public sealed record CreateCategoryRequest(
    [Required, MaxLength(80)] string Name,
    CategoryType Type,
    [MaxLength(20)] string? Color = null,
    [MaxLength(50)] string? Icon = null);
