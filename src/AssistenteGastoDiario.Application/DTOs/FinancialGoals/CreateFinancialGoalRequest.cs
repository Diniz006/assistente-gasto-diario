using System.ComponentModel.DataAnnotations;
using AssistenteGastoDiario.Domain.Enums;

namespace AssistenteGastoDiario.Application.DTOs.FinancialGoals;

public sealed record CreateFinancialGoalRequest(
    Guid? CategoryId,
    [Required, MaxLength(140)] string Name,
    [Range(0.01, 9999999999.99)] decimal TargetAmount,
    [Range(0, 9999999999.99)] decimal CurrentAmount = 0m,
    [Range(0, 9999999999.99)] decimal MonthlyPlannedAmount = 0m,
    DateOnly? TargetDate = null,
    GoalPriority Priority = GoalPriority.Medium,
    GoalStatus Status = GoalStatus.Active,
    string? Notes = null);
