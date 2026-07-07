using System.ComponentModel.DataAnnotations;
using AssistenteGastoDiario.Domain.Enums;

namespace AssistenteGastoDiario.Application.DTOs.FinancialGoals;

public sealed record UpdateFinancialGoalRequest(
    Guid? CategoryId,
    [Required, MaxLength(140)] string Name,
    [Range(0.01, 9999999999.99)] decimal TargetAmount,
    [Range(0, 9999999999.99)] decimal CurrentAmount,
    [Range(0, 9999999999.99)] decimal MonthlyPlannedAmount,
    DateOnly? TargetDate,
    GoalPriority Priority,
    GoalStatus Status,
    string? Notes);
