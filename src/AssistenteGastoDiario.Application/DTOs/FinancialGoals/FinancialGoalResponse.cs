using AssistenteGastoDiario.Domain.Enums;

namespace AssistenteGastoDiario.Application.DTOs.FinancialGoals;

public sealed record FinancialGoalResponse(
    Guid Id,
    Guid UserId,
    Guid? CategoryId,
    string Name,
    decimal TargetAmount,
    decimal CurrentAmount,
    decimal MonthlyPlannedAmount,
    DateOnly? TargetDate,
    GoalPriority Priority,
    GoalStatus Status,
    string? Notes,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
